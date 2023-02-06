using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zk.HotelPlatform.Model;

namespace Zk.HotelPlatform.Service
{
    public interface ISysUserGroupUserService
    {
        IEnumerable<SysUserGroupUser> GetUserGroups(int uid);
        IEnumerable<SysUserGroupUser> GetSysUserGroupUsers(int groupId);
        bool SaveUserGroups(int userId, IEnumerable<int> groupIds, int? optUserId = null);
        bool SaveSysUserGroupUsers(int groupId, IEnumerable<int> sysUserGroupUsers, int userId);
    }
}
