using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zk.HotelPlatform.Model
{
    [Table("Sys_UserGroup_User")]
    public class SysUserGroupUser
    {
        [Key]
        public int Id { get; set; }
        public int UserId { get; set; }
        public int UserGroupId { get; set; }
    }
}
