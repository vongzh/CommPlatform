﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;
using TencentCloud.Tbaas.V20180416.Models;
using Zk.HotelPlatform.DBProvider;
using Zk.HotelPlatform.Model;
using Zk.HotelPlatform.Model.Basic.Pager;
using Zk.HotelPlatform.Model.Request;
using Zk.HotelPlatform.Utils;
using Zk.HotelPlatform.Utils.Global;
using Senparc.CO2NET.Extensions;

namespace Zk.HotelPlatform.Service.Impl
{
    public class OrderInfoService : DataService<OrderInfo>, IOrderInfoService
    {
        private readonly ISignupService _signupService = null;
        private readonly ICourseService _courseService = null;
        private readonly ISchemeService _schemeService = null;
        private readonly IPaymentOrderService _paymentOrderService = null;

        public OrderInfoService(IPaymentOrderService paymentOrderService,ICourseService courseService, ISchemeService schemeService, ISignupService signupService, IDataProvider<OrderInfo> dataProvider) : base(dataProvider)
        {
            this._signupService = signupService;
            this._courseService = courseService;
            this._schemeService = schemeService;
            _paymentOrderService = paymentOrderService;
        }

        /// <summary>
        /// 查询订单
        /// </summary>
        /// <param name="orderNo"></param>
        /// <returns></returns>
        public OrderInfo GetOrderInfo(string orderNo)
        {
            return base.Get(x => x.OrderNo == orderNo);
        }

        /// <summary>
        /// 查询订单列表
        /// </summary>
        /// <param name="queryRequest"></param>
        /// <returns></returns>
        public PageResult<OrderInfo> QueryOrders(QueryOrderRequest queryRequest)
        {
            Expression<Func<OrderInfo, bool>> filter = x => x.Id > 0;
            if (!string.IsNullOrWhiteSpace(queryRequest.OrderNo))
            {
                filter = filter.And(x => x.OrderNo == queryRequest.OrderNo.Trim());
            }
            if (queryRequest.CreateTimeBegin.HasValue)
            {
                filter = filter.And(x => x.CreateTime >= queryRequest.CreateTimeBegin.ToBeginDate());
            }
            if (queryRequest.CreateTimeEnd.HasValue)
            {
                filter = filter.And(x => x.CreateTime >= queryRequest.CreateTimeEnd.ToEndDate());
            }
            IOrderedQueryable<OrderInfo> orderby(IQueryable<OrderInfo> x) => x.OrderByDescending(z => z.Id);

            var orders = base.Select(out int totalCount, filter, queryRequest.PageIndex, queryRequest.PageSize, orderby);
            if (orders == null || orders.Count() == 0)
                return new PageResult<OrderInfo>()
                {
                    Rows = new List<OrderInfo>(),
                    Total = 0
                };

            return new PageResult<OrderInfo>()
            {
                Total = totalCount,
                Rows = orders
            };
        }

        /// <summary>
        /// 创建订单
        /// </summary>
        /// <param name="createOrderRequest"></param>
        /// <returns></returns>
        /// <exception cref="BusinessException"></exception>
        /// <exception cref="NotImplementedException"></exception>
        public bool CerateOrder(CreateOrderRequest createOrderRequest)
        {
            var course = _courseService.GetCourse(createOrderRequest.CourseId);
            if (course == null)
                throw new BusinessException("未找到课程信息");
            var scheme = _schemeService.GetScheme(createOrderRequest.SchemeId);
            if (scheme == null)
                throw new BusinessException("未找到方案信息");

            var o = base.Get(x => x.OpenId == createOrderRequest.Signup.OpenId);
            if (o != null)
                throw new BusinessException("已经报过名了");

            var signupInfo = createOrderRequest.Signup;
            if (!_signupService.SaveSignup(signupInfo))
                throw new BusinessException("提交失败");

            var orderInfo = new OrderInfo()
            {
                OrderNo = DateTime.Now.Ticks.ToString(),
                CourseId = course.Id,
                CourseName = course.CourseName,
                Duration = course.Duration,
                Price = course.Price,
                SchemeId = scheme.Id,
                SchemeName = scheme.SchemeName,
                ServiceRate = scheme.ServiceRate,
                SchemeNum = scheme.TotalNumber,
                SchemePayNum = 0,
                Status = (int)PartalEnum.OrderStatus.WAIT_PAY,
                PaymentAmount = 0,
                PaymentServiceAmount = 0,
                TotalAmount = Convert.ToInt32(course.Price * (1 + scheme.ServiceRate / 100)),
                TotalServiceAmount = Convert.ToInt32(course.Price * (scheme.ServiceRate / 100)),
                UserId = signupInfo.Id,
                UserName = signupInfo.Name,
                BeginClassTime = createOrderRequest.BeginClassTime,
                OpenId = signupInfo.OpenId
            };

            if (orderInfo.Id <= 0)
            {
                orderInfo.CreateTime = DateTime.Now;
                orderInfo.IsDelete = (int)GlobalEnum.YESOrNO.N;
                var entity = AddAndGetEntity(orderInfo);

                var ret = entity != null && entity.Id > 0;
                if (ret)
                    _paymentOrderService.CreatePaymentOrders(orderInfo);

                return ret;
            }
            else
                throw new NotImplementedException();
        }

        public bool UpdatePayment(OrderInfo orderInfo) 
        {
            var order = Get(x => x.OrderNo == orderInfo.OrderNo);
            order.PaymentAmount = orderInfo.PaymentAmount;
            order.PaymentServiceAmount = orderInfo.PaymentServiceAmount;
            order.SchemePayNum = orderInfo.SchemePayNum;
            order.Status = orderInfo.Status;
            return base.Update(order);
        }
    }
}
