using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zk.HotelPlatform.Model;

namespace Zk.HotelPlatform.Service
{
    public interface ISysUserRoleService
    {
        bool SaveUserRoles(int userId, IEnumerable<int> roles, int? optUserId = null);
        IEnumerable<SysUserRole> GetUserRoles(int userId);
        IEnumerable<SysUserRole> DeleteUserRoles(IEnumerable<int> Ids);
    }
}
