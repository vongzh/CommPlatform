using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApiThrottle;

namespace Zk.HotelPlatform.CacheProvider
{
    public interface IThrottleCacheProvider
    {
        ThrottlePolicy GetThrottlePolicy();
        bool SetThrottlePolicy(ThrottlePolicy throttlePolicy);
        bool RemoveThrottlePolicy();
        bool ExistThrottle(string dataKey);
        bool RemoveThrottle();
        bool RemoveThrottle(string id);
        ThrottleCounter? GetThrottle(string id);
        bool SetThrottle(string id, ThrottleCounter throttleCounter, TimeSpan expirationTime);
    }
}
