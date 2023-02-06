using Microsoft.Owin;
using System;
using System.Linq;
using System.Net.Http;

namespace Zk.HotelPlatform.Utils
{
    public static class RequestUtil
    {
        private const string HttpContext = "MS_HttpContext";
        private const string RemoteEndpointMessage = "System.ServiceModel.Channels.RemoteEndpointMessageProperty";
        private const string OwinContext = "MS_OwinContext";

        private const string Real_IP = "X-Real-IP";
        private const string ForwardedFor = "X-Forwarded-For";

        public static string GetClientIp(this IOwinRequest request)
        {
            try
            {
                if (request.Headers.ContainsKey(Real_IP))
                {
                    return request.Headers.GetValues(Real_IP).FirstOrDefault();
                }

                if (request.Headers.ContainsKey(ForwardedFor))
                {
                    return request.Headers.GetValues(ForwardedFor).FirstOrDefault();
                }

                if (!string.IsNullOrWhiteSpace(request.RemoteIpAddress))
                {
                    return request.RemoteIpAddress;
                }

                return "0.0.0.0";
            }
            catch (Exception ex)
            {
                return "0.0.0.0";
            }
        }

        public static string GetClientIp(this HttpRequestMessage request)
        {
            try
            {
                if (request.Headers.Contains(Real_IP))
                {
                    return request.Headers.GetValues(Real_IP).FirstOrDefault();
                }

                if (request.Headers.Contains(ForwardedFor))
                {
                    return request.Headers.GetValues(ForwardedFor).FirstOrDefault();
                }

                if (request.Properties.ContainsKey(HttpContext))
                {
                    dynamic ctx = request.Properties[HttpContext];
                    if (ctx != null)
                    {
                        return ctx.Request.UserHostAddress;
                    }
                }

                // Self-hosting. Needs reference to System.ServiceModel.dll. 
                if (request.Properties.ContainsKey(RemoteEndpointMessage))
                {
                    dynamic remoteEndpoint = request.Properties[RemoteEndpointMessage];
                    if (remoteEndpoint != null)
                    {
                        return remoteEndpoint.Address;
                    }
                }

                // Self-hosting using Owin. Needs reference to Microsoft.Owin.dll. 
                if (request.Properties.ContainsKey(OwinContext))
                {
                    dynamic owinContext = request.Properties[OwinContext];
                    if (owinContext != null)
                    {
                        return owinContext.Request.RemoteIpAddress;
                    }
                }
                return "0.0.0.0";
            }
            catch (Exception)
            {
                return "0.0.0.0";
            }
        }
    }
}
