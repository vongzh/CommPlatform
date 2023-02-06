using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zk.HotelPlatform.Model
{
    public class BaseSysMenu
    {
        public int Id { get; set; }
        public int ParentId { get; set; }
        public string Title { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Component { get; set; }
        public string Path { get; set; }
        public string Redirect { get; set; }
        public string Icon { get; set; }
        public int Sort { get; set; }
        public int Level { get; set; }
        public int Enabled { get; set; }
        public int Visiable { get; set; }
        public int BreadcrumbShow { get; set; }
        public int AlwaysShow { get; set; }
    }
}
