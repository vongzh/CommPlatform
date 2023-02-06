
using Newtonsoft.Json;
using Owin;
using Senparc.CO2NET.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Zk.HotelPlatform.CacheModel;
using Zk.HotelPlatform.Model;
using Zk.HotelPlatform.Service;
using Zk.HotelPlatform.Utils;
using Zk.HotelPlatform.Utils.Global;
using Zk.HotelPlatform.Utils.Log;

namespace Zk.HotelPlatform.Api.Filters
{
    public class OperationLogAttribute : ActionFilterAttribute
    {
        private const string _sysUserSessionContext = GlobalConst.SYSUSER_SESSION_CONTEXT;
        private const string _logContext = GlobalConst.LOG_CONTEXT;
        private const string _logName = "RequestLog";

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            try
            {
                SysUserSession sysUserSession = null;
                if (OwinRequestScopeContext.Current.Items.ContainsKey(_sysUserSessionContext))
                {
                    sysUserSession = (SysUserSession)OwinRequestScopeContext.Current.Items[_sysUserSessionContext];
                }

                var operationLogService = (IOperationLogService)actionContext.RequestContext.Configuration.DependencyResolver.GetService(typeof(IOperationLogService));
                var log = new RequestLog()
                {
                    LogId = Guid.NewGuid().ToString("N"),
                    ClientDevice = actionContext.Request.Headers.UserAgent.ToString(),
                    ClientIp = actionContext.Request.GetClientIp(),
                    RequestPath = actionContext.Request.RequestUri.LocalPath,
                    RequestParam = JsonConvert.SerializeObject(actionContext.ActionArguments),
                    UserName = sysUserSession?.UserName,
                    UserId = sysUserSession?.UserId.ToString(),
                    UserType = sysUserSession?.UserType,
                    RequestTime = DateTime.Now,
                };
                OwinRequestScopeContext.Current.Items[_logContext] = log;
            }
            catch (Exception ex)
            {

            }
            finally
            {
                base.OnActionExecuting(actionContext);
            }
        }

        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {

            if (OwinRequestScopeContext.Current.Items.ContainsKey(_logContext))
            {
                var log = (RequestLog)OwinRequestScopeContext.Current.Items[_logContext];
                if (log != null)
                {
                    if (actionExecutedContext.Response != null && actionExecutedContext.Response.Content != null)
                    {
                        var res = actionExecutedContext.Response.Content.ReadAsStringAsync().Result;
                        log.ResponseResult = res;
                    }
                    LogInfoWriter.GetInstance(_logName).Info(JsonConvert.SerializeObject(log));
                }
            }
            base.OnActionExecuted(actionExecutedContext);
        }
    }

    public class RequestLog
    {
        public string LogId { get; set; }
        public string ClientIp { get; set; }
        public string ClientDevice { get; set; }
        public string RequestPath { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public int? UserType { get; set; }
        public string RequestParam { get; set; }
        public string ResponseResult { get; set; }
        public DateTime RequestTime { get; set; }
        public DateTime? ResponseTime { get; set; }
    }
}
