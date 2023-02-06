using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zk.HotelPlatform.Model
{
    [Table("Sys_UserGroup_Module")]
    public class SysUserGroupModule
    {
        [Key]
        public int Id { get; set; }
        public int ModuleId { get; set; }
        public int UserGroupId { get; set; }
    }
}
