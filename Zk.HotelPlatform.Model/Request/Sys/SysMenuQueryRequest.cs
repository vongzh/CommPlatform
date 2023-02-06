using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zk.HotelPlatform.Model.Request
{
    public class SysMenuQueryRequest
    {
        public string Component { get; set; }
        public string Path { get; set; }
        public string Title { get; set; }
        public int? ParentId { get; set; }
        public int? Enabled { get; set; }
        public int? Level { get; set; }
    }
}
