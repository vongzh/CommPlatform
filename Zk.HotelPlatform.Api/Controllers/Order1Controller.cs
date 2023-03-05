using Nest;
using SKIT.FlurlHttpClient.Wechat.TenpayV3.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.UI.WebControls;
using TencentCloud.Cpdp.V20190820.Models;
using Zk.HotelPlatform.Api.Filters;
using Zk.HotelPlatform.Model;
using Zk.HotelPlatform.Model.Basic.Pager;
using Zk.HotelPlatform.Model.Request;
using Zk.HotelPlatform.Service;
using Zk.HotelPlatform.Utils.Log;
using CreateOrderRequest = Zk.HotelPlatform.Model.Request.CreateOrderRequest;
using QueryOrderRequest = Zk.HotelPlatform.Model.Request.QueryOrderRequest;

namespace Zk.HotelPlatform.Api.Controllers
{
#if !DEBUG
    [Authorize(Roles = "Client")]
#endif
    [ResponseHandler]
    public class Order1Controller : BaseController
    {
        private readonly IOrderInfoService _orderInfoService = null;
        private readonly IPaymentOrderService _paymentOrderService = null;
        private readonly ISignupService _signupService = null;

        public Order1Controller(ISignupService signupService, IPaymentOrderService paymentOrderService, IOrderInfoService orderInfoService)
        {
            _orderInfoService = orderInfoService;
            _paymentOrderService = paymentOrderService;
            _signupService = signupService;
        }

        [HttpPost]
        [SysAuthorize]
        [Route("Order1/Query")]
        public PageResult<OrderInfo> QueryOrders(QueryOrderRequest queryOrderRequest)
        {
            if (queryOrderRequest == null)
                queryOrderRequest = new QueryOrderRequest();

            return this._orderInfoService.QueryOrders(queryOrderRequest);
        }

        [HttpPost]
        [SysAuthorize]
        [Route("Order1/Get")]
        public OrderInfo GetOrder(string orderNo)
        {
            return this._orderInfoService.GetOrderInfo(orderNo);
        }

        [HttpPost]
        [SysAuthorize]
        [Route("Signup/Get")]
        public Signup GetSignup(int userId)
        {
            return _signupService.Get(x => x.Id == userId);
        }

        [HttpPost]
        [Route("Order1/Create")]
        public bool CerateOrder([FromUri] string openId, [FromBody] CreateOrderRequest createOrderRequest)
        {
            if (createOrderRequest.Signup == null)
                throw new ArgumentException(nameof(createOrderRequest.Signup));
            if (string.IsNullOrWhiteSpace(createOrderRequest.Signup.Name))
                throw new ArgumentException(nameof(createOrderRequest.Signup.Name));
            if (string.IsNullOrWhiteSpace(createOrderRequest.Signup.QQ))
                throw new ArgumentException(nameof(createOrderRequest.Signup.QQ));
            if (string.IsNullOrWhiteSpace(createOrderRequest.Signup.Wechat))
                throw new ArgumentException(nameof(createOrderRequest.Signup.Wechat));
            if (string.IsNullOrWhiteSpace(createOrderRequest.Signup.Email))
                throw new ArgumentException(nameof(createOrderRequest.Signup.Email));
            if (string.IsNullOrWhiteSpace(createOrderRequest.Signup.Educational))
                throw new ArgumentException(nameof(createOrderRequest.Signup.Educational));
            if (string.IsNullOrWhiteSpace(createOrderRequest.Signup.UrgentContactPersonAddress))
                throw new ArgumentException(nameof(createOrderRequest.Signup.UrgentContactPersonAddress));
            if (string.IsNullOrWhiteSpace(createOrderRequest.Signup.UrgentContactPersonName))
                throw new ArgumentException(nameof(createOrderRequest.Signup.UrgentContactPersonName));
            if (string.IsNullOrWhiteSpace(createOrderRequest.Signup.UrgentContactPersonPhone))
                throw new ArgumentException(nameof(createOrderRequest.Signup.UrgentContactPersonPhone));
            if (string.IsNullOrWhiteSpace(createOrderRequest.Signup.UrgentContactPersonRelation))
                throw new ArgumentException(nameof(createOrderRequest.Signup.UrgentContactPersonRelation));

            if (createOrderRequest.CourseId <= 0)
                throw new ArgumentException(nameof(createOrderRequest.CourseId));
            if (createOrderRequest.SchemeId <= 0)
                throw new ArgumentException(nameof(createOrderRequest.SchemeId));

            if (string.IsNullOrWhiteSpace(createOrderRequest.Signup.OpenId))
            {
                createOrderRequest.Signup.OpenId = openId;
            }
            return _orderInfoService.CerateOrder(createOrderRequest);
        }

        [HttpPost]
        [Route("Payment/GetPaymentOrders")]
        public IEnumerable<PaymentOrder> GetPaymentOrders([FromUri] string orderNo, [FromUri] string openId)
        {
            return _paymentOrderService.GetPaymentOrders(orderNo, openId);
        }

        [HttpPost]
        [Route("Payment/Pay")]
        public async Task<IDictionary<string, string>> Payment([FromBody] string[] paymentOrderNos, [FromUri] string openId)
        {
            return await _paymentOrderService.Payment(paymentOrderNos, openId);
        }

        [Route("Payment/TenpayCallback")]
        [HttpPost]
        public async Task<dynamic> TenpayCallback()
        {
            try
            {
                string timestamp = Request.Headers.TryGetValues("Wechatpay-Timestamp", out _) ? Request.Headers.GetValues("Wechatpay-Timestamp").First() : null;
                string nonce = Request.Headers.TryGetValues("Wechatpay-Nonce", out _) ? Request.Headers.GetValues("Wechatpay-Nonce").First() : null;
                string signature = Request.Headers.TryGetValues("Wechatpay-Signature", out _) ? Request.Headers.GetValues("Wechatpay-Signature").First() : null;
                string serialNumber = Request.Headers.TryGetValues("Wechatpay-Serial", out _) ? Request.Headers.GetValues("Wechatpay-Serial").First() : null;

                using (var stream = await Request.Content.ReadAsStreamAsync())
                using (var reader = new StreamReader(stream, Encoding.UTF8))
                {
                    string content = await reader.ReadToEndAsync();

                    {
                        LogInfoWriter.GetInstance().Warn($"{timestamp}\r\n{nonce}\r\n{signature}\r\n{serialNumber}\r\n{content}");
                    }

                    var ret = await _paymentOrderService.TenpayCallback(timestamp, nonce, content, signature, serialNumber);
                    if (ret)
                        return new
                        {
                            code = "SUCCESS",
                            message = "成功"
                        };
                    else
                        return new
                        {
                            code = "FAIL",
                            message = "失败"
                        };
                }
            }
            catch (Exception ex)
            {
                return new
                {
                    code = "FAIL",
                    message = ex.Message
                };
            }
        }
    }
}
