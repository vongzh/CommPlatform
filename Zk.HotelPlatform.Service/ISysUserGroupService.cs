using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zk.HotelPlatform.Model.Request;
using Zk.HotelPlatform.Model.Response;

namespace Zk.HotelPlatform.Service
{
    public interface ISysUserGroupService
    {
        SysUserGroupResponse GetUserGroup(int id);
        SysUserGroupResponse GetUserGroup(string meta);
        IEnumerable<SysUserGroupResponse> GetUserGroups();
        IEnumerable<SysUserGroupResponse> GetUserGroups(IEnumerable<int> ids);
        IEnumerable<SysUserGroupResponse> GetUserGroups(SysUserGroupQueryRequest queryRequest);
        bool SaveUserGroup(SysUserGroupAddRequest addRequest, int userId);
        bool Delete(int id, int userId);
    }
}
