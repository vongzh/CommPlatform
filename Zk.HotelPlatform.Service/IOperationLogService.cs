using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zk.HotelPlatform.CacheModel;
using Zk.HotelPlatform.Model;
using Zk.HotelPlatform.Model.Basic.Pager;
using Zk.HotelPlatform.Model.Request;
using Zk.HotelPlatform.Model.Response.Log;

namespace Zk.HotelPlatform.Service
{
    public interface IOperationLogService
    {
        bool AddLog(OperationLog operationLog);
        bool AddOperationLog(string optType, string businessParam, SysUserSession sysUser);
        PageResult<OperationLogResponse> GetOperationLogs(LogQueryRequest queryRequest);
        PageResult<OperationLogResponse> GetOperationLogs(LogQueryRequest queryRequest, int userId);
    }
}
