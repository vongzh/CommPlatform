using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zk.HotelPlatform.Model;
using Zk.HotelPlatform.Model.Request;
using Zk.HotelPlatform.Model.Response;

namespace Zk.HotelPlatform.Service
{
    public interface ISysMenuService
    {
        IEnumerable<SysMenuResponse> GetMenus(SysMenuQueryRequest queryRequest);
        SysMenuResponse GetMenu(int menuId);
        bool SaveMenu(SysMenuAddRequest request, int userId);
        IEnumerable<SysMenuResponse> GetNestMenus();
        bool Enabled(int id, int userId);
        bool Delete(int id, int userId);
        IEnumerable<SysMenuResponse> GetMenus(IEnumerable<int> menuIds);
        IEnumerable<SysMenu> GetMenus();
    }
}
