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
using Senparc.Weixin.MP.AdvancedAPIs.MerChant;

namespace Zk.HotelPlatform.Service.Impl
{
    public class PaymentOrderService : DataService<PaymentOrder>, IPaymentOrderService
    {
        private readonly IMapper _mapper = null;
        private readonly TencentPayService _tencentPayService = null;

        public IOrderInfoService OrderInfoService { get; set; }

        private const string _sequenceName = "Seq_PaymentOrderNo";

        public PaymentOrderService(IMapper mapper, IDataProvider<PaymentOrder> dataProvider)
            : base(dataProvider)
        {
            _mapper = mapper;
            _tencentPayService = new TencentPayService();
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
        public async Task<IDictionary<string, string>> Payment(string[] paymentNos, string openId)
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
            return await _tencentPayService.CreateOrderByJsApi(requestData);
        }

        /// <summary>
        /// 腾讯支付回调
        /// </summary>
        /// <returns></returns>
        public async Task<bool> TenpayCallback(string timestamp, string nonce, string content, string signature, string serialNumber)
        {
            var transcationResource = await _tencentPayService.PaymentCallBack(timestamp, nonce, content, signature, serialNumber);
            if (transcationResource == null) return false;

            var paymentOrders = base.Select(x => x.PaymentSerialNo == transcationResource.OutTradeNumber).ToList();

            var orderNo = paymentOrders.FirstOrDefault().OrderNo;
            var orderInfo = OrderInfoService.GetOrderInfo(orderNo);
            if (orderInfo == null)
                throw new BusinessException("订单不存在");

            if (transcationResource.TradeState == "SUCCESS")
            {
                foreach (var paymentOrder in paymentOrders)
                {
                    paymentOrder.TransactionId = transcationResource.TransactionId;
                    paymentOrder.PaymentStatus = (int)PartalEnum.PaymentOrderStatus.PAIED;
                    paymentOrder.PaymentTime = transcationResource.SuccessTime.LocalDateTime;
                }
                UpdateAll(paymentOrders);

                {
                    orderInfo.PaymentAmount += paymentOrders.Sum(x => x.TotalAmount);
                    orderInfo.PaymentServiceAmount += paymentOrders.Sum(x => x.ServiceAmount);
                    orderInfo.SchemePayNum += paymentOrders.Count;
                    if (orderInfo.SchemePayNum == orderInfo.SchemeNum)
                        orderInfo.Status = (int)PartalEnum.OrderStatus.COMPLATE;
                    else
                        orderInfo.Status = (int)PartalEnum.OrderStatus.PART_PAY;
                    orderInfo.Status = orderInfo.Status;
                    OrderInfoService.UpdatePayment(orderInfo);
                }
            }
            return false;
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
                for (int i = 1; i <= orderInfo.SchemeNum; i++)
                {
                    var paymentOrderNo = GetSequence(_sequenceName);
                    paymentOrders.Add(new PaymentOrder()
                    {
                        Periods = i,
                        ClassAmount = orderInfo.Price / orderInfo.SchemeNum,
                        CreateTime = DateTime.Now,
                        IsDelete = (int)GlobalEnum.YESOrNO.N,
                        OrderNo = orderInfo.OrderNo,
                        UserId = orderInfo.UserId,
                        TotalAmount = orderInfo.TotalAmount / orderInfo.SchemeNum,
                        PaymentNo = paymentOrderNo.PadTo().ToString(),
                        OpenId = orderInfo.OpenId,
                        PaymentStatus = (int)PartalEnum.PaymentOrderStatus.WAIT_PAY,
                        ServiceAmount = orderInfo.TotalServiceAmount / orderInfo.SchemeNum,
                        PayableTime = DateTime.Now.AddMonths(i)
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
