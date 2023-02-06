using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zk.HotelPlatform.Model.Request;
using Zk.HotelPlatform.Model.Response;

namespace Zk.HotelPlatform.Service
{
    public interface ISysRoleService
    {
        SysRoleResponse GetRole(int id);
        SysRoleResponse GetRole(string meta);
        bool SaveRole(SysRoleAddRequest addRequest,int userId);
        bool Delete(int id, int userId);
        IEnumerable<SysRoleResponse> GetRoles();
        IEnumerable<SysRoleResponse> GetRoles(SysRoleQueryRequest queryRequest);
        IEnumerable<SysRoleResponse> GetRoles(IEnumerable<int> ids);
    }
}
