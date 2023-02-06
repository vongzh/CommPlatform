using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zk.HotelPlatform.Model
{
    [Table("Sys_Config")]
    public class SysConfig
    {
        public int Id { get; set; }
        public string Domain { get; set; }
        public string Group { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
        public string Memo { get; set; }
        public string KeyName { get; set; }
        public string ValueName { get; set; }
        public int IsDelete { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime? ModifyTime { get; set; }
        public int Creator{get;set;}
        public int? Modifer { get; set; }
    }
}
