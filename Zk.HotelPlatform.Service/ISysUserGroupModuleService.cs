using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zk.HotelPlatform.Model;

namespace Zk.HotelPlatform.Service
{
    public interface ISysUserGroupModuleService
    {
        IEnumerable<SysUserGroupModule> GetUserGroupModules(IEnumerable<int> groupIds);
        IEnumerable<SysUserGroupModule> GetUserGroupModules(int groupId);
        bool SaveUserGroupModule(int groupId, IEnumerable<int> groupModules, int userId);
    }
}
