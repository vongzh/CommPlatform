using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApiThrottle;

namespace Zk.HotelPlatform.Service
{
    public interface IThrottleService
    {
        ThrottlePolicy GetThrottlePolicy();
        bool SetThrottlePolicy(ThrottlePolicy throttlePolicy, int sysUserId);
    }
}
