using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Zk.HotelPlatform.DBProvider;
using Zk.HotelPlatform.Model;
using Zk.HotelPlatform.Model.Basic.Pager;
using Zk.HotelPlatform.Model.Request;
using Zk.HotelPlatform.Utils;

namespace Zk.HotelPlatform.Service.Impl
{
    public class LoginLogService : DataService<LoginLog>, ILoginLogService
    {
        public LoginLogService(IDataProvider<LoginLog> dataProvider) : base(dataProvider)
        {
        }

        public bool Add(LoginLog loginLog)
        {
            loginLog.LoginTime = DateTime.Now;
            return base.AddEntity(loginLog);
        }

        public PageResult<LoginLog> GetLoginLogs(LogQueryRequest queryRequest)
        {
            Expression<Func<LoginLog, bool>> filter = x => x.Id > 0;
            if (!string.IsNullOrWhiteSpace(queryRequest.UserName))
            {
                filter = filter.And(x => x.UserName == queryRequest.UserName.Trim());
            }
            if (queryRequest.Date.BeginTime.HasValue && queryRequest.Date.EndTime.HasValue)
            {
                var bDate = queryRequest.Date.BeginTime.ToBeginDate();
                var eDate = queryRequest.Date.EndTime.ToEndDate();

                filter = filter.And(x => x.LoginTime >= bDate && x.LoginTime <= eDate);
            }

            IOrderedQueryable<LoginLog> orderby(IQueryable<LoginLog> x) => x.OrderByDescending(z => z.Id);

            var logs = base.Select(out int totalCount, filter, queryRequest.PageIndex, queryRequest.PageSize, orderby);
            if (logs == null || logs.Count() == 0)
                return new PageResult<LoginLog>()
                {
                    Rows = new List<LoginLog>(),
                    Total = 0
                };

            return new PageResult<LoginLog>()
            {
                Total = totalCount,
                Rows = logs
            };
        }
    }
}
