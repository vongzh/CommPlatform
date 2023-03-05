using Aop.Api.Domain;
using Aop.Api.Util;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using static Zk.HotelPlatform.Utils.BusinessException;
using Zk.HotelPlatform.DBProvider;
using Zk.HotelPlatform.Model;
using Zk.HotelPlatform.Utils.Global;
using Zk.HotelPlatform.Utils;
using Senparc.Weixin.Entities;
using Zk.HotelPlatform.Utils.Log;
using TencentCloud.Tbaas.V20180416.Models;
using SKIT.FlurlHttpClient.Wechat.TenpayV3.Models;
using TencentCloud.Cpdp.V20190820.Models;

namespace Zk.HotelPlatform.Service.Impl
{
    public class PaymentOrderService : DataService<PaymentOrder>, IPaymentOrderService
    {
        private readonly IMapper _mapper = null;
        private readonly TencentPayService tencentPayService = null;

        public IOrderInfoService OrderInfoService { get; set; }

        private const string _sequenceName = "Seq_PaymentOrderNo";

        public PaymentOrderService(IMapper mapper, IDataProvider<PaymentOrder> dataProvider)
            : base(dataProvider)
        {
            _mapper = mapper;
            tencentPayService = new TencentPayService();
        }

        /// <summary>
        /// 查询支付单
        /// </summary>
        /// <param name="orderNo"></param>
        /// <param name="openId"></param>
        /// <returns></returns>
        public IEnumerable<PaymentOrder> GetPaymentOrders(string orderNo,string openId)
        {
            Expression<Func<PaymentOrder, bool>> filter = x => x.OpenId == openId;
            if (!string.IsNullOrWhiteSpace(orderNo))
            {
                filter = filter.And(x => x.OrderNo == orderNo);
            }
            return base.Select(filter);
        }

        /// <summary>
        /// 支付
        /// </summary>
        /// <returns></returns>
        public async Task<CreatePayTransactionJsapiResponse> Payment(string[] paymentNos, string openId)
        {
            Expression<Func<PaymentOrder, bool>> filter = x => x.OpenId == openId;
            if (paymentNos != null && paymentNos.Length > 0)
            {
                filter = filter.And(x => paymentNos.Contains(x.PaymentNo));
            }

            var paymentOrders = base.Select(filter).ToList();
            if (paymentOrders.Any(x => x.PaymentStatus != (int)PartalEnum.PaymentOrderStatus.WAIT_PAY))
                throw new BusinessException("支付订单状态不正确");

            var orderNo = paymentOrders.FirstOrDefault().OrderNo;
            var orderInfo = OrderInfoService.GetOrderInfo(orderNo);
            if (orderInfo == null) 
                throw new BusinessException("订单不存在");

            var serialNo = $"{DateTime.Now.Ticks}{openId.Substring(0, 4)}";
            foreach (var paymentOrder in paymentOrders)
            {
                paymentOrder.PaymentSerialNo = serialNo;
                paymentOrder.ModifyTime = DateTime.Now;
            }
            UpdateAll(paymentOrders);
            //BulkUpdate(paymentOrders);

            var totalAmount = paymentOrders.Sum(x => x.TotalAmount);
            var requestData = new CreatePayTransactionJsapiRequest()
            {
                OutTradeNumber = serialNo,
                Description = $"{orderInfo.CourseName}",
                Amount = new CreatePayTransactionJsapiRequest.Types.Amount() { Total = totalAmount.Value },
                Payer = new CreatePayTransactionJsapiRequest.Types.Payer() { OpenId = orderInfo.OpenId }
            };
            return await tencentPayService.CreateOrderByJsApi(requestData);
        }

        /// <summary>
        /// 创建支付单
        /// </summary>
        /// <param name="orderNo"></param>
        /// <param name="userId"></param>
        /// <param name="paymentInfos"></param>
        /// <returns></returns>
        public void CreatePaymentOrders(OrderInfo orderInfo)
        {
            try
            {
                if (orderInfo == null)
                    throw new OuterBusinessException("订单不存在");
                if (orderInfo.Status == (int)PartalEnum.OrderStatus.COMPLATE)
                    throw new OuterBusinessException("订单已经完成");

                var paymentOrders = new List<PaymentOrder>();
                for (int i = 0; i < orderInfo.SchemeNum; i++)
                {
                    var paymentOrderNo = GetSequence(_sequenceName);
                    paymentOrders.Add(new PaymentOrder()
                    {
                        Periods = i,
                        ClassAmount = orderInfo.Price,
                        CreateTime = DateTime.Now,
                        IsDelete = (int)GlobalEnum.YESOrNO.N,
                        OrderNo = orderInfo.OrderNo,
                        UserId = orderInfo.UserId,
                        TotalAmount = orderInfo.PaymentAmount,
                        PaymentNo = paymentOrderNo.PadTo().ToString(),
                        OpenId = orderInfo.OpenId,
                        PaymentStatus = (int)PartalEnum.PaymentOrderStatus.WAIT_PAY,
                        ServiceAmount = orderInfo.PaymentServiceAmount,
                    });
                }
                base.InsertAll(paymentOrders);
                //BulkInsert(paymentOrders);
            }
            catch (Exception ex)
            {
                LogInfoWriter.GetInstance().Error(ex);
            }
        }
    }
}
