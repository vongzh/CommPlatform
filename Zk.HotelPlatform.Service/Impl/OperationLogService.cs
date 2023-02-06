using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Zk.HotelPlatform.CacheModel;
using Zk.HotelPlatform.DBProvider;
using Zk.HotelPlatform.Model;
using Zk.HotelPlatform.Model.Basic.Pager;
using Zk.HotelPlatform.Model.Request;
using Zk.HotelPlatform.Model.Response.Log;
using Zk.HotelPlatform.Utils;
using Zk.HotelPlatform.Utils.Global;

namespace Zk.HotelPlatform.Service.Impl
{
    public class OperationLogService : DataService<OperationLog>, IOperationLogService
    {
        private IMapper _mapper = null;
        public OperationLogService(IMapper mapper, IDataProvider<OperationLog> dataProvider) : base(dataProvider)
        {
            _mapper = mapper;
        }

        public bool AddLog(OperationLog operationLog)
        {
            operationLog.LogId = Guid.NewGuid().ToString("N");
            operationLog.OptTime = DateTime.Now;
            return base.AddEntity(operationLog);
        }

        public bool AddOperationLog(string optType, string businessParam, SysUserSession sysUser)
        {
            var log = new OperationLog()
            {
                UserId = sysUser.UserId.ToString(),
                UserName = sysUser.NickName,
                UserType = sysUser.UserType,
                BusinessParam = businessParam,
            };

            switch (optType)
            {
                case "CUSTOMER_IDCARD_VIEW":
                    log.LogContent = "客户身份证照片预览";
                    log.ResourceType = (int)GlobalEnum.ResourceType.CUSTOMER_IDCARD_VIEW;
                    break;
                case "CUSTOMER_BUSINES_VIEW":
                    log.LogContent = "客户营业执照照片预览";
                    log.ResourceType = (int)GlobalEnum.ResourceType.CUSTOMER_BUSINES_VIEW;
                    break;
                case "EXPORT_ALLORDER":
                    log.LogContent = "全部订单导出";
                    log.ResourceType = (int)GlobalEnum.ResourceType.EXPORT_ALLORDER;
                    break;
                case "EXPORT_SYSTEMORDER":
                    log.LogContent = "系统订单导出";
                    log.ResourceType = (int)GlobalEnum.ResourceType.EXPORT_SYSTEMORDER;
                    break;
                case "EXPORT_WAITSYSTEMORDER":
                    log.LogContent = "系统待办订单导出";
                    log.ResourceType = (int)GlobalEnum.ResourceType.EXPORT_WAITSYSTEMORDER;
                    break;
                case "EXPORT_MUANALORDER":
                    log.LogContent = "人工订单导出";
                    log.ResourceType = (int)GlobalEnum.ResourceType.EXPORT_MUANALORDER;
                    break;
                case "EXPORT_WAITMUANALORDER":
                    log.LogContent = "人工待办订单导出";
                    log.ResourceType = (int)GlobalEnum.ResourceType.EXPORT_WAITMUANALORDER;
                    break;
                case "EXPORT_REFUNDORDER":
                    log.LogContent = "退款列表导出";
                    log.ResourceType = (int)GlobalEnum.ResourceType.EXPORT_REFUNDORDER;
                    break;
                case "EXPORT_ACCOUNTTRAN":
                    log.LogContent = "账户交易列表导出";
                    log.ResourceType = (int)GlobalEnum.ResourceType.EXPORT_ACCOUNTTRAN;
                    break;
                case "EXPORT_PURCHASEORDER":
                    log.LogContent = "采购单列表导出";
                    log.ResourceType = (int)GlobalEnum.ResourceType.EXPORT_PURCHASEORDER;
                    break;
                case "CUSTOMER_EXPORT_ALLORDER":
                    log.LogContent = "客户订单导出";
                    log.ResourceType = (int)GlobalEnum.ResourceType.CUSTOMER_EXPORT_ALLORDER;
                    break;
                case "CUSTOMER_EXPORT_REFUNDORDER":
                    log.LogContent = "客户退款列表导出";
                    log.ResourceType = (int)GlobalEnum.ResourceType.CUSTOMER_EXPORT_REFUNDORDER;
                    break;
                case "CUSTOMER_EXPORT_ACCOUNTTRAN":
                    log.LogContent = "客户账户交易列表导出";
                    log.ResourceType = (int)GlobalEnum.ResourceType.CUSTOMER_EXPORT_ACCOUNTTRAN;
                    break;
                default:
                    return true;
            }
            return this.AddLog(log);
        }

        public PageResult<OperationLogResponse> GetOperationLogs(LogQueryRequest queryRequest, int userId)
        {
            string uId = userId.ToString();
            Expression<Func<OperationLog, bool>> filter = x => x.UserId == uId;
            var now = DateTime.Now;
            var endDate = now.Date.ToEndDate();
            var beginDate = now.Date.AddDays(-3);
            filter = filter.And(x => x.OptTime >= beginDate && x.OptTime <= endDate);

            IOrderedQueryable<OperationLog> orderby(IQueryable<OperationLog> x) => x.OrderByDescending(z => z.OptTime).ThenByDescending(y => y.Id);

            var logs = base.Select(out int totalCount, filter, queryRequest.PageIndex, queryRequest.PageSize, orderby);
            if (logs == null || logs.Count() == 0)
                return new PageResult<OperationLogResponse>()
                {
                    Rows = new List<OperationLogResponse>(),
                    Total = 0
                };

            var logResponses = _mapper.Map<IEnumerable<OperationLog>, IEnumerable<OperationLogResponse>>(logs);
            foreach (var logResponse in logResponses)
            {
                logResponse.UserTypeDesc = GlobalEnum.GetEnumDescription((GlobalEnum.UserType)logResponse.UserType);
                logResponse.ResourceTypeDesc = GlobalEnum.GetEnumDescription((GlobalEnum.ResourceType)logResponse.ResourceType);
            }

            return new PageResult<OperationLogResponse>()
            {
                Total = totalCount,
                Rows = logResponses
            };
        }

        public PageResult<OperationLogResponse> GetOperationLogs(LogQueryRequest queryRequest)
        {
            Expression<Func<OperationLog, bool>> filter = x => x.Id > 0;
            if (!string.IsNullOrWhiteSpace(queryRequest.LogId))
            {
                filter = filter.And(x => x.LogId == queryRequest.LogId);
            }
            if (!string.IsNullOrWhiteSpace(queryRequest.UserName))
            {
                filter = filter.And(x => x.UserName == queryRequest.UserName);
            }
            if (!string.IsNullOrWhiteSpace(queryRequest.BusinessParam))
            {
                filter = filter.And(x => x.BusinessParam == queryRequest.BusinessParam);
            }
            if (queryRequest.Date.BeginTime.HasValue && queryRequest.Date.EndTime.HasValue)
            {
                var bDate = queryRequest.Date.BeginTime.ToBeginDate();
                var eDate = queryRequest.Date.EndTime.ToEndDate();

                filter = filter.And(x => x.OptTime >= bDate && x.OptTime <= eDate);
            }

            IOrderedQueryable<OperationLog> orderby(IQueryable<OperationLog> x) => x.OrderByDescending(z => z.Id);

            var logs = base.Select(out int totalCount, filter, queryRequest.PageIndex, queryRequest.PageSize, orderby);
            if (logs == null || logs.Count() == 0)
                return new PageResult<OperationLogResponse>()
                {
                    Rows = new List<OperationLogResponse>(),
                    Total = 0
                };

            var logResponses = _mapper.Map<IEnumerable<OperationLog>, IEnumerable<OperationLogResponse>>(logs);
            foreach (var logResponse in logResponses)
            {
                logResponse.UserTypeDesc = GlobalEnum.GetEnumDescription((GlobalEnum.UserType)logResponse.UserType);
                logResponse.ResourceTypeDesc = GlobalEnum.GetEnumDescription((GlobalEnum.ResourceType)logResponse.ResourceType);
            }

            return new PageResult<OperationLogResponse>()
            {
                Total = totalCount,
                Rows = logResponses
            };
        }
    }
}
