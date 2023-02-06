using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zk.HotelPlatform.Model.Base.Sys;

namespace Zk.HotelPlatform.Model.Response
{
    public class SysDepartmentResponse : BaseSysDepartment
    {
        public int Id { get; set; }
        public DateTime? SyncTime { get; set; }

        public SysDepartmentResponse ParentDepartment { get; set; }
    }
}
