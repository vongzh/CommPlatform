using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zk.HotelPlatform.Model
{
    [Table("Sys_Module")]
    public class SysModule : BaseEntity
    {
        public int Control { get; set; }
        public int? Type { get; set; }
        [StringLength(20)]
        public string Meta { get; set; }
        [StringLength(100)]
        public string Path { get; set; }
        [StringLength(20)]
        public string ModuleName { get; set; }
        [StringLength(100)]
        public string Description { get; set; }
    }
}
