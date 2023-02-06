using System.Web.Http;
using WebApiThrottle;
using Zk.HotelPlatform.Api.Filters;
using Zk.HotelPlatform.Service;

namespace Zk.HotelPlatform.Api.Controllers
{
    [SysAuthorize]
    [ResponseHandler]
    [Authorize(Roles = "Client")]
    public class ThrottleController : BaseController
    {
        private readonly IThrottleService _throttleService = null;
        public ThrottleController(IThrottleService throttleService)
        {
            _throttleService = throttleService;
        }

        
        [HttpPost]
        [Route("Throttle/Policy")]
        public ThrottlePolicy ThrottlePolicy()
        {
            return _throttleService.GetThrottlePolicy();
        }

        
        [HttpPost]
        [Route("Throttle/SetPolicy")]
        public bool SetThrottlePolicy(ThrottlePolicy throttlePolicy)
        {
            return _throttleService.SetThrottlePolicy(throttlePolicy, base.CurrentSysUser.UserId);
        }
    }
}
