using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Filters;
using Zk.HotelPlatform.CacheModel;
using Zk.HotelPlatform.Model.Basic.Response;
using Zk.HotelPlatform.Utils;
using Zk.HotelPlatform.Utils.Global;

namespace Zk.HotelPlatform.Api.Filters
{
    public class ResponseHandlerAttribute : ActionFilterAttribute
    {
        private const string _sysUserSession = GlobalConst.SYSUSER_SESSION_CONTEXT;

        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            if (actionExecutedContext.ActionContext.ActionDescriptor.GetCustomAttributes<ResponseHandlerIgnoreAttribute>().Any())
            {
                goto baseExcute;
            }

            if (actionExecutedContext.Exception != null)
                goto baseExcute;

            if (actionExecutedContext.Response != null && actionExecutedContext.Response.Content != null)
            {
                if (actionExecutedContext.Response.StatusCode == HttpStatusCode.Unauthorized)
                    goto baseExcute;

                ResponseInfo<object> result = new ResponseInfo<object>()
                {
                    Result = true,
                    Message = string.Empty,
                    Data = actionExecutedContext.ActionContext.Response.Content.ReadAsAsync<object>().Result
                };

                var responseContent = JsonConvert.SerializeObject(result, Settings.JsonSerializerSettings);

#if DEBUG
                actionExecutedContext.Response = new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(content: responseContent, encoding: Encoding.UTF8, mediaType: "application/json")
                };
#else
                OwinRequestScopeContext.Current.Items.TryGetValue(_sysUserSession, out object session);
                if (session != null)
                {
                    var userSession = (SysUserSession)session;
                    responseContent = AESUtil.AesEncrypt(responseContent, Md5Util.Md5Encrypt(userSession.PrivateKey), userSession.Seed);
                    actionExecutedContext.Response = new HttpResponseMessage()
                    {
                        StatusCode = HttpStatusCode.OK,
                        Content = new StringContent(content: responseContent, encoding: Encoding.UTF8, mediaType: "text/plain")
                    };
                }
                else
                {
                    actionExecutedContext.Response = new HttpResponseMessage()
                    {
                        StatusCode = HttpStatusCode.OK,
                        Content = new StringContent(content: responseContent, encoding: Encoding.UTF8, mediaType: "application/json")
                    };
                }
#endif
            }
            else
            {
                actionExecutedContext.Response = new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.NoContent
                };
            }

        baseExcute:
            base.OnActionExecuted(actionExecutedContext);
        }
    }
}
