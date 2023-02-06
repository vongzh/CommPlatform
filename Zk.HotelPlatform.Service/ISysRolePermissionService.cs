using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zk.HotelPlatform.Model;

namespace Zk.HotelPlatform.Service
{
    public interface ISysRolePermissionService
    {
        IEnumerable<SysRolePermission> GetRolePermissions(int roleId);
        bool SaveRolePermission(int roleId, IEnumerable<int> permissionIds, int userId);
        IEnumerable<SysRolePermission> GetRolePermissions(IEnumerable<int> roleIds);
    }
}
