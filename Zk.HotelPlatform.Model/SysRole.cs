using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zk.HotelPlatform.Model
{
    [Table("Sys_Role")]
    public class SysRole : BaseEntity
    {
        [StringLength(20)]
        public string Meta { get; set; }
        [StringLength(20)]
        public string RoleName { get; set; }
        [StringLength(100)]
        public string Description { get; set; }
    }
}
