using Microsoft.Extensions.Caching.Memory;
using RedisClientAchieve;
using System;
using System.Collections.Generic;
using Zk.HotelPlatform.CacheModel;
//using Zk.HotelPlatform.Utils.Cache;

namespace Zk.HotelPlatform.CacheProvider.Impl
{
    public class SysUserCacheProvider : ISysUserCacheProvider
    {
        #region Session
        private const string _sysUserSessionKey = "HotelPlatform:SysUserSession";

        public bool SetSession(string dataKey, SysUserSession sysUserSession)
        {
            return RedisEntities.Default.HashSet<SysUserSession>(_sysUserSessionKey, dataKey, sysUserSession, TimeSpan.FromDays(7));
            //var ret = RedisUtil.Hash_Set<SysUserSession>(_sysUserSessionKey, dataKey, sysUserSession, RedisCacheConfig.DBId);
            //if (ret)
            //{
            //    RedisUtil.SetExpire(_sysUserSessionKey, DateTime.Now.AddDays(7), RedisCacheConfig.DBId);
            //}
            //return ret;
        }

        public SysUserSession GetSession(string dataKey)
        {
            return RedisEntities.Default.HashGet<SysUserSession>(_sysUserSessionKey, dataKey);
            //return RedisUtil.Hash_Get<SysUserSession>(_sysUserSessionKey, dataKey, RedisCacheConfig.DBId);
        }

        public bool RemoveSession(string dataKey)
        {
            return RedisEntities.Default.HashRemoveField(_sysUserSessionKey, dataKey);
            //return RedisUtil.Hash_Remove(_sysUserSessionKey, dataKey, RedisCacheConfig.DBId);
        }
        #endregion

        #region Token
        private const string _sysUserTokenKey = "HotelPlatform:SysUserToken";

        public bool SetToken(string dataKey, string token)
        {
            return RedisEntities.Default.HashSet(_sysUserTokenKey, dataKey, token, TimeSpan.FromDays(7));
            //var ret = RedisUtil.Hash_Set(_sysUserTokenKey, dataKey, token, RedisCacheConfig.DBId);
            //if (ret)
            //{
            //    RedisUtil.SetExpire(_sysUserTokenKey, DateTime.Now.AddDays(7), RedisCacheConfig.DBId);
            //}
            //return ret;
        }

        public string GetToken(string dataKey)
        {
            //return RedisUtil.Hash_Get(_sysUserTokenKey, dataKey, RedisCacheConfig.DBId);
            return RedisEntities.Default.HashGet<string>(_sysUserTokenKey, dataKey);
        }

        public bool RemoveToken(string dataKey)
        {
            //return RedisUtil.Hash_Remove(_sysUserTokenKey, dataKey, RedisCacheConfig.DBId);
            return RedisEntities.Default.HashRemoveField(_sysUserTokenKey, dataKey);
        }

        #endregion
    }

    public class SysUserMemoryCacheProvider : ISysUserCacheProvider
    {
        private readonly IMemoryCache _memoryCache;

        public SysUserMemoryCacheProvider(IMemoryCache memoryCache) => _memoryCache = memoryCache;

        #region Session
        private const string _sysUserSessionKey = "HotelPlatform:SysUserSession";

        private static readonly object _obj = new object();

        private static readonly object _obj1 = new object();

        public bool SetSession(string dataKey, SysUserSession sysUserSession)
        {
            lock (_obj)
            {
                var dataDic = _memoryCache.Get<Dictionary<string, SysUserSession>>(_sysUserSessionKey);
                if (dataDic is null)
                    dataDic = new Dictionary<string, SysUserSession>();
                if (dataDic.ContainsKey(dataKey))
                    dataDic.Remove(dataKey);

                dataDic.Add(dataKey, sysUserSession);

                _memoryCache.Set(_sysUserSessionKey, dataDic);

                return true;
            }
        }

        public SysUserSession GetSession(string dataKey)
        {
            var dataDic = _memoryCache.Get<Dictionary<string, SysUserSession>>(_sysUserSessionKey);
            if (dataDic is null) return null;

            return dataDic.ContainsKey(dataKey) ? dataDic[dataKey] : null;
        }

        public bool RemoveSession(string dataKey)
        {
            lock (_obj)
            {
                var dataDic = _memoryCache.Get<Dictionary<string, SysUserSession>>(_sysUserSessionKey);
                if (dataDic is null) return true;

                if (dataDic.ContainsKey(dataKey))
                    dataDic.Remove(dataKey);

                _memoryCache.Set(_sysUserSessionKey, dataDic);

                return true;
            }
        }
        #endregion

        #region Token
        private const string _sysUserTokenKey = "HotelPlatform:SysUserToken";

        public bool SetToken(string dataKey, string token)
        {
            lock (_obj)
            {
                var dataDic = _memoryCache.Get<Dictionary<string, string>>(_sysUserTokenKey);
                if (dataDic is null)
                    dataDic = new Dictionary<string, string>();

                if (dataDic.ContainsKey(dataKey))
                    dataDic.Remove(dataKey);

                dataDic.Add(dataKey, token);

                _memoryCache.Set(_sysUserTokenKey, dataDic);

                return true;
            }
        }

        public string GetToken(string dataKey)
        {
            var dataDic = _memoryCache.Get<Dictionary<string, string>>(_sysUserTokenKey);
            if (dataDic is null) return null;

            return dataDic.ContainsKey(dataKey) ? dataDic[dataKey] : null;
        }

        public bool RemoveToken(string dataKey)
        {
            lock (_obj)
            {
                var dataDic = _memoryCache.Get<Dictionary<string, string>>(_sysUserTokenKey);
                if (dataDic is null) return true;

                if (dataDic.ContainsKey(dataKey))
                    dataDic.Remove(dataKey);

                _memoryCache.Set(_sysUserTokenKey, dataDic);

                return true;
            }
        }

        #endregion
    }

    public class SysUserSessionCompare : IEqualityComparer<SysUserSession>
    {
        public static SysUserSessionCompare Instance = new SysUserSessionCompare();

        public bool Equals(SysUserSession x, SysUserSession y)
        {
            return x.UserId == y.UserId && x.Id == y.Id;
        }

        public int GetHashCode(SysUserSession obj)
        {
            return obj.GetHashCode();
        }
    }
}
