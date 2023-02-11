using System;
using System.Collections.Generic;
using System.Web.Http;
using Zk.HotelPlatform.Api.Filters;
using Zk.HotelPlatform.Model;
using Zk.HotelPlatform.Model.Basic.Pager;
using Zk.HotelPlatform.Model.Request;
using Zk.HotelPlatform.Service;

namespace Zk.HotelPlatform.Api.Controllers
{
#if !DEBUG
    [Authorize(Roles = "Client")]
#endif
    [ResponseHandler]
    public class OrderController : BaseController
    {
        private readonly IOrderInfoService _orderInfoService = null;
        public OrderController(IOrderInfoService orderInfoService)
        {
            _orderInfoService = orderInfoService;
        }

        [HttpPost]
        [SysAuthorize]
        [Route("Order/Query")]
        public PageResult<OrderInfo> QueryOrders(QueryOrderRequest queryOrderRequest)
        {
            if (queryOrderRequest == null)
                queryOrderRequest = new QueryOrderRequest();
            
            return this._orderInfoService.QueryOrders(queryOrderRequest);
        }

        [HttpPost]
        [SysAuthorize]
        [Route("Order/Get")]
        public OrderInfo GetOrder(string orderNo)
        {
            return this._orderInfoService.GetOrderInfo(orderNo);
        }

        [HttpPost]
        [Route("Order/Create")]
        public bool CerateOrder(CreateOrderRequest createOrderRequest)
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
            if (createOrderRequest.BeginClassTime <= DateTime.Now)
                throw new ArgumentException(nameof(createOrderRequest.BeginClassTime));
            return _orderInfoService.CerateOrder(createOrderRequest);
        }
    }
}
