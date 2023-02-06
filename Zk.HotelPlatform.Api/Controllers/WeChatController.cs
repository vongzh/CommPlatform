using System.Web.Http;
using Zk.HotelPlatform.Api.Filters;
using Zk.HotelPlatform.Service;

namespace Zk.HotelPlatform.Api.Controllers
{
    [ResponseHandler]
    [Authorize(Roles = "Client")]
    public class WeChatController : BaseController
    {
        private readonly IWeChatService _weChatService = null;

        public WeChatController(IWeChatService weChatService)
        {
            _weChatService = weChatService;
        }
    }
}
