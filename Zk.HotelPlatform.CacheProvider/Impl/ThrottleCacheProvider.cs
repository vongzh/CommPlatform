using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApiThrottle;
//using Zk.HotelPlatform.Utils.Cache;
using RedisClientAchieve;

namespace Zk.HotelPlatform.CacheProvider.Impl
{
    public class ThrottleCacheProvider : IThrottleCacheProvider
    {
        private const string _policyKey = "HotelPlatform:ThrottlePolicy";
        private const string _throttleKey = "HotelPlatform:ThrottleCounter";

        #region Policy
        public ThrottlePolicy GetThrottlePolicy()
        {
            //return RedisUtil.Get<ThrottlePolicy>(_policyKey, RedisCacheConfig.DBId);
            return RedisEntities.Default.ItemGet<ThrottlePolicy>(_policyKey);
        }

        public bool SetThrottlePolicy(ThrottlePolicy throttlePolicy)
        {
            //return RedisUtil.Set<ThrottlePolicy>(_policyKey, throttlePolicy, RedisCacheConfig.DBId);
            return RedisEntities.Default.ItemSet<ThrottlePolicy>(_policyKey, throttlePolicy);
        }

        public bool RemoveThrottlePolicy()
        {
            return RedisEntities.Default.ItemRemove(_policyKey);
        }
        #endregion

        #region Throttle
        public bool ExistThrottle(string dataKey)
        {
            return RedisEntities.Default.HashExistField(_throttleKey, dataKey);
        }

        public bool RemoveThrottle()
        {
            return RedisEntities.Default.HashRemove(_throttleKey);
        }

        public bool RemoveThrottle(string id)
        {
            return RedisEntities.Default.HashRemoveField(_throttleKey, id);
        }

        public ThrottleCounter? GetThrottle(string id)
        {
            return RedisEntities.Default.HashGet<ThrottleCounter>(_throttleKey, id);
        }

        public bool SetThrottle(string id, ThrottleCounter throttleCounter, TimeSpan expirationTime)
        {
            return RedisEntities.Default.HashSet(_throttleKey, id, throttleCounter, expirationTime);
        }
        #endregion
    }
}
