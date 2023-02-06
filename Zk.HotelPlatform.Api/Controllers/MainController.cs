using System.Web.Http;
using Zk.HotelPlatform.Service;

namespace Zk.HotelPlatform.Api.Controllers
{
    /// <summary>
    /// 基本信息
    /// </summary>
    public class MainController : BaseController
    {
        private readonly IMainService _mainService = null;

        public MainController(IMainService mainService)
        {
            this._mainService = mainService;
        }

        /// <summary>
        /// 基本信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        
        public dynamic Index()
        {
            return _mainService.GetServerInfo();
        }
    }
}
