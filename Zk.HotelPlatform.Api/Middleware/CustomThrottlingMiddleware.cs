using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using WebApiThrottle;
using WebApiThrottle.Net;
using Zk.HotelPlatform.Utils;

namespace Zk.HotelPlatform.Api.Middleware
{
    public class CustomThrottlingMiddleware : ThrottlingMiddleware
    {
        private const string _sessionId = "sid";
        private const string _quotaExceededMessage = "API calls quota exceeded!";

        private readonly List<string> ipWhiteList = new List<string>()
        {
            //"127.0.0.1",
            //"192.168"
        };

        private readonly List<string> endpointWhiteList = new List<string>()
        {
            "/signalr",
            "/token",
            "/sysuser",
        };

        public CustomThrottlingMiddleware(OwinMiddleware next) : base(next)
        {
            base.QuotaExceededMessage = _quotaExceededMessage;
        }

        public CustomThrottlingMiddleware(OwinMiddleware next, ThrottlePolicy policy, IPolicyRepository policyRepository,
            IThrottleRepository repository, IThrottleLogger logger, IIpAddressParser ipAddressParser)
            : base(next, policy, policyRepository, repository, logger, ipAddressParser)
        {
            base.QuotaExceededMessage = _quotaExceededMessage;
        }

        protected override RequestIdentity SetIdentity(IOwinRequest request)
        {
            string clientKey = request.Headers.GetValues(_sessionId).FirstOrDefault();
            string endpoint = request.Uri.AbsolutePath.ToLowerInvariant();
            string clientIp = request.GetClientIp();

            var requestIdentity = new RequestIdentity()
            {
                ClientIp = clientIp,
                ClientKey = clientKey,
                Endpoint = endpoint,
                ForceWhiteList = false
            };

            //Client Key
            if (!request.Headers.ContainsKey(_sessionId))
            {
                requestIdentity.ForceWhiteList = true;

                return requestIdentity;
            }

            //Endpoint
            if (endpointWhiteList.Contains(endpoint))
            {
                requestIdentity.ForceWhiteList = true;

                return requestIdentity;
            }
            if (endpointWhiteList.Any(ep => endpoint.Contains(ep)))
            {
                requestIdentity.ForceWhiteList = true;

                return requestIdentity;
            }

            //IP
            if (ipWhiteList.Contains(clientIp))
            {
                requestIdentity.ForceWhiteList = true;

                return requestIdentity;
            }
            if (ipWhiteList.Any(ip => clientIp.Contains(ip)))
            {
                requestIdentity.ForceWhiteList = true;

                return requestIdentity;
            }

            return requestIdentity;
        }

        public override Task Invoke(IOwinContext context)
        {
            if (!context.Response.Headers.ContainsKey("Access-Control-Expose-Headers"))
            {
                context.Response.Headers.Set("Access-Control-Expose-Headers", "*");
            }
            if (!context.Response.Headers.ContainsKey("Access-Control-Request-Headers"))
            {
                context.Response.Headers.Set("Access-Control-Request-Headers", "*");
            }
            if (!context.Response.Headers.ContainsKey("Access-Control-Request-Method"))
            {
                context.Response.Headers.Set("Access-Control-Request-Method", "*");
            }
            if (!context.Response.Headers.ContainsKey("Access-Control-Allow-Origin"))
            {
                context.Response.Headers.Set("Access-Control-Allow-Origin", "*");
            }

            return base.Invoke(context);
        }
    }
}
