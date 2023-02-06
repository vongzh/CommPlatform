using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zk.HotelPlatform.Model
{
    public class BaseSysModule
    {
        public int Control { get; set; }
        public int Type { get; set; }
        public string Meta { get; set; }
        public string ModuleName { get; set; }
        public string Path { get; set; }
        public string Description { get; set; }
    }
}
