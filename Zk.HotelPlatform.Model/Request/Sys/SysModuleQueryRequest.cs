using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zk.HotelPlatform.Model.Basic.Pager;

namespace Zk.HotelPlatform.Model.Request
{
    public class SysModuleQueryRequest: PageRequest
    {
        public string ModuleName { get; set; }
        public string Meta { get; set; }
    }
}
