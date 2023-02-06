using Newtonsoft.Json;
using System;
using WebApiThrottle;
using Zk.HotelPlatform.CacheProvider;
using Zk.HotelPlatform.Model;
//using Zk.HotelPlatform.Utils.Cache;
using Zk.HotelPlatform.Utils.Global;
using Zk.HotelPlatform.Utils.Log;

namespace Zk.HotelPlatform.Service.Impl
{
    public class ThrottleService : IThrottleService
    {
        private readonly ISysUserService _sysUserService = null;
        private readonly IOperationLogService _operationLogService = null;
        private readonly IThrottleCacheProvider _throttleCacheProvider = null;

        public ThrottleService(ISysUserService sysUserService, IOperationLogService operationLogService,
            IThrottleCacheProvider throttleCacheProvider)
        {
            _throttleCacheProvider = throttleCacheProvider;
            _operationLogService = operationLogService;
            _sysUserService = sysUserService;
        }

        public ThrottlePolicy GetThrottlePolicy()
        {
            try
            {
                var policy = _throttleCacheProvider.GetThrottlePolicy();
                if (policy == null)
                {
                    policy = ThrottlePolicy.FromStore(new PolicyConfigurationProvider());
                }
                return policy;
            }
            catch (Exception ex)
            {
                LogInfoWriter.GetInstance().Error(ex);
                
                return null;
            }
        }

        public bool SetThrottlePolicy(ThrottlePolicy throttlePolicy, int sysUserId)
        {
            var currentPolicy = this.GetThrottlePolicy();

            var ret = _throttleCacheProvider.SetThrottlePolicy(throttlePolicy);
            if (ret)
            {
                LogInfoWriter.GetInstance().Info($"系统限流配置:{JsonConvert.SerializeObject(currentPolicy)}{throttlePolicy}");

                ThrottleManager.UpdatePolicy(throttlePolicy, new PolicyRepository(_throttleCacheProvider));

                var sysUser = _sysUserService.GetUserInfoByUserId(sysUserId);
                _operationLogService.AddLog(new OperationLog()
                {
                    UserId = sysUserId.ToString(),
                    UserName = sysUser.NickName,
                    UserType = sysUser.UserType,
                    LogContent = "新增/修改系统限流配置",
                    ChangedFields = JsonConvert.SerializeObject(throttlePolicy),
                    ResourceType = (int)GlobalEnum.ResourceType.Config_Throttle,
                });
            }
            return ret;
        }
    }

    public class PolicyRepository : IPolicyRepository
    {
        //private const string _cacheKey = "HotelPlatform:ThrottlePolicy";
        //private const int _redisDbId = 5;

        private readonly IThrottleCacheProvider _throttleCacheProvider = null;
        public PolicyRepository(IThrottleCacheProvider throttleCacheProvider)
        {
            _throttleCacheProvider = throttleCacheProvider;
        }

        public ThrottlePolicy FirstOrDefault(string id)
        {
            return _throttleCacheProvider.GetThrottlePolicy();
        }

        public void Remove(string id)
        {
            //RedisUtil.Remove(_cacheKey, _redisDbId);
            _throttleCacheProvider.RemoveThrottlePolicy();
        }

        public void Save(string id, ThrottlePolicy policy)
        {
            //RedisUtil.Set(_cacheKey, policy, _redisDbId);
            _throttleCacheProvider.SetThrottlePolicy(policy);
        }
    }

    public class ThrottleRepository : IThrottleRepository
    {
        //private const string _cacheKey = "HotelPlatform:ThrottleCounter";
        //private const int _redisDbId = 5;

        private readonly IThrottleCacheProvider _throttleCacheProvider = null;
        public ThrottleRepository(IThrottleCacheProvider throttleCacheProvider)
        {
            _throttleCacheProvider = throttleCacheProvider;
        }

        public bool Any(string id)
        {
            //return RedisUtil.Hash_Exist<ThrottleCounter>(_cacheKey, id, _redisDbId);
            return _throttleCacheProvider.ExistThrottle(id);
        }

        public void Clear()
        {
            //RedisUtil.Hash_Remove(_cacheKey, _redisDbId);
            _throttleCacheProvider.RemoveThrottle();
        }

        public ThrottleCounter? FirstOrDefault(string id)
        {
            return _throttleCacheProvider.GetThrottle(id);
            //return RedisUtil.Hash_Get<ThrottleCounter>(_cacheKey, id, _redisDbId);

        }

        public void Remove(string id)
        {
            //RedisUtil.Hash_Remove(_cacheKey, id, _redisDbId);
            _throttleCacheProvider.RemoveThrottle(id);
        }

        public void Save(string id, ThrottleCounter throttleCounter, TimeSpan expirationTime)
        {
            //RedisUtil.Hash_Set(_cacheKey, id, throttleCounter, _redisDbId);
            _throttleCacheProvider.SetThrottle(id, throttleCounter, expirationTime);
        }
    }

    public class ThrottleLogger : IThrottleLogger
    {
        private const string _logName = "Throttle";

        public void Log(ThrottleLogEntry entry)
        {
            LogInfoWriter.GetInstance(_logName).Info(entry);
        }
    }
}
