using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zk.HotelPlatform.Model
{
    [Table("Sys_Menu")]
    public class SysMenu : BaseEntity
    {
        public int ParentId { get; set; }
        [StringLength(20)]
        public string Title { get; set; }
        [StringLength(50)]
        public string Name { get; set; }
        [StringLength(50)]
        public string Description { get; set; }
        [StringLength(20)]
        public string Icon { get; set; }
        [StringLength(50)]
        public string Component { get; set; }
        [StringLength(100)]
        public string Path { get; set; }
        [StringLength(100)]
        public string Redirect { get; set; }
        public int Visiable { get; set; }
        public int Sort { get; set; }
        public int Level { get; set; }
        public int Enabled { get; set; }
        public int AlwaysShow { get; set; }
        public int BreadcrumbShow { get; set; }
    }
}
