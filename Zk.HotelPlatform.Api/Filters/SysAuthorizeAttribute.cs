using Newtonsoft.Json;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Zk.HotelPlatform.Model;
using Zk.HotelPlatform.Service;
using Zk.HotelPlatform.Utils;
using Zk.HotelPlatform.Utils.Global;

namespace Zk.HotelPlatform.Api.Filters
{
    public class SysAuthorizeAttribute : AuthorizationFilterAttribute
    {
        private const string _clientToken = "ck";
        private const string _sessionId = "sid";

        private const string _reasonPhrase = "Self";

        private const string _sysUserSession = GlobalConst.SYSUSER_SESSION_CONTEXT;

        private readonly Dictionary<int, string> _errDic = new Dictionary<int, string>()
        {
            { -1,"禁止访问: -10010" },
            { -2,"禁止访问: -10011" },
        };

        /// <summary>
        /// 用户权限验证
        /// </summary>
        /// <param name="actionContext"></param>
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            if (actionContext.ActionDescriptor.GetCustomAttributes<AllowAnonymousAttribute>().Any())
                return;

            if (!actionContext.Request.Headers.Contains(_clientToken) || !actionContext.Request.Headers.Contains(_sessionId))
            {
                actionContext.Response = actionContext.Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "User Not Authorization");
                actionContext.Response.Headers.Add("Access-Control-Expose-Headers", "*");
                actionContext.Response.ReasonPhrase = _reasonPhrase;
                return;
            }

            var sessionId = actionContext.Request.Headers.GetValues(_sessionId).FirstOrDefault();
            var sysUserSession = ((ISysUserService)actionContext.RequestContext.Configuration.DependencyResolver.GetService(typeof(ISysUserService))).GetUserSessionCache(sessionId);
            if (sysUserSession == null || sysUserSession.UserId <= 0)
            {
                actionContext.Response = actionContext.Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "User Not Login");
                actionContext.Response.Headers.Add("Access-Control-Expose-Headers", "*");
                actionContext.Response.ReasonPhrase = _reasonPhrase;
                return;
            }
            string sysUerToken = ((ISysUserService)actionContext.RequestContext.Configuration.DependencyResolver.GetService(typeof(ISysUserService))).GetUserTokenCache(sysUserSession.UserId.ToString());
            if (string.IsNullOrWhiteSpace(sysUerToken))
            {
                actionContext.Response = actionContext.Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "User Token Error");
                actionContext.Response.Headers.Add("Access-Control-Expose-Headers", "*");
                actionContext.Response.ReasonPhrase = _reasonPhrase;
                return;
            }

            string clientToken = actionContext.Request.Headers.GetValues(_clientToken).FirstOrDefault();
            if (string.IsNullOrWhiteSpace(clientToken))
            {
                actionContext.Response = actionContext.Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "Client Request Error");
                actionContext.Response.Headers.Add("Access-Control-Expose-Headers", "*");
                actionContext.Response.ReasonPhrase = _reasonPhrase;
                return;
            }

            string clientSecret = AESUtil.AesDecrypt(clientToken, Md5Util.Md5Encrypt(sysUserSession.PrivateKey), sysUserSession.Seed);
            ClientToken clientData = JsonConvert.DeserializeObject<ClientToken>(clientSecret,
            new JsonSerializerSettings()
            {
                DateTimeZoneHandling = DateTimeZoneHandling.Local
            });

            if (DateTime.TryParse(clientData.tm, out DateTime reqTime))
            {
                if ((DateTime.Now - reqTime).Seconds > 10)
                {
                    actionContext.Response = actionContext.Request
               .CreateResponse<string>(HttpStatusCode.Forbidden, _errDic[-1]);
                    actionContext.Response.Headers.Add("Access-Control-Expose-Headers", "*");
                    actionContext.Response.ReasonPhrase = _reasonPhrase;
                    return;
                }
            }
            else
            {
                actionContext.Response = actionContext.Request
               .CreateResponse<string>(HttpStatusCode.Forbidden, _errDic[-2]);
                actionContext.Response.Headers.Add("Access-Control-Expose-Headers", "*");
                actionContext.Response.ReasonPhrase = _reasonPhrase;
                return;
            }
            OwinRequestScopeContext.Current.Items.Add(_sysUserSession, sysUserSession);

            base.OnAuthorization(actionContext);
        }
    }

    public class ClientToken
    {
        public string sid { get; set; }
        public string tm { get; set; }
    }
}
