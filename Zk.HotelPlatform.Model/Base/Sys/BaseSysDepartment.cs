using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zk.HotelPlatform.Model.Base.Sys
{
    public class BaseSysDepartment
    {
        public string DepartmentName { get; set; }
        public int? ParentId { get; set; }
        public long Order { get; set; }
    }
}
