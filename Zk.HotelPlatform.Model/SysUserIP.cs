using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zk.HotelPlatform.Model
{
    [Table("Sys_UserIP")]
    public class SysUserIP
    {
        [Key]
        public int Id { get; set; }
        public int UserId { get; set; }
        public string IPWhiteList { get; set; }
        public string IPBlackList { get; set; }
        public int IsDelete { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime? ModifyTime { get; set; }
        public int Creater { get; set; }
        public int? Modifier { get; set; }
    }
}
