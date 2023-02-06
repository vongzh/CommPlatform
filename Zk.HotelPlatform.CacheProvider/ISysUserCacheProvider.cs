using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zk.HotelPlatform.CacheModel;

namespace Zk.HotelPlatform.CacheProvider
{
    public interface ISysUserCacheProvider
    {
        bool SetSession(string token, SysUserSession sysUserSession);
        SysUserSession GetSession(string token);
        bool RemoveSession(string token);
        bool SetToken(string userId, string token);
        string GetToken(string userId);
        bool RemoveToken(string userId);
    }
}
