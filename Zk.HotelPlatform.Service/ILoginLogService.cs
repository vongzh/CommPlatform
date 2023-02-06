using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zk.HotelPlatform.Model;
using Zk.HotelPlatform.Model.Basic.Pager;
using Zk.HotelPlatform.Model.Request;

namespace Zk.HotelPlatform.Service
{
    public interface ILoginLogService
    {
        bool Add(LoginLog loginLog);
        PageResult<LoginLog> GetLoginLogs(LogQueryRequest queryRequest);
    }
}
