using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zk.HotelPlatform.Model
{
    [Table("Sys_Department")]
    public class SysDepartment : BaseEntity
    {
        public long? ThirdId { get; set; }
        public string DepartmentName { get; set; }
        public int? ParentId { get; set; }
        public long? ThirdParentId { get; set; }
        public long Order { get; set; }
        public DateTime? SyncTime { get; set; }
    }
}
