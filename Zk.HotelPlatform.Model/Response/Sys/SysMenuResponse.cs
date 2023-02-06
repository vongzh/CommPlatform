using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zk.HotelPlatform.Model.Request;

namespace Zk.HotelPlatform.Model.Response
{
    public class SysMenuResponse : BaseSysMenu
    {
        public SysMenuResponse Parent { get; set; }
        public IEnumerable<SysMenuResponse> SubMenus { get; set; }
    }
}
