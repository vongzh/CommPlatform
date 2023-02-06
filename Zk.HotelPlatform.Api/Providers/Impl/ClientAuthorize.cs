using System.Security.Claims;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace Zk.HotelPlatform.Api.Providers.Impl
{
    public class ClientAuthorize : AuthorizeAttribute
    {
        public const string Client = "Client";

        public ClientAuthorize()
        {
            base.Roles = Client;
        }

        protected override bool IsAuthorized(HttpActionContext actionContext)
        {
            return base.IsAuthorized(actionContext);
        }
    }
}