using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zk.HotelPlatform.Model;
using Zk.HotelPlatform.Model.Request;
using Zk.HotelPlatform.Model.Response;

namespace Zk.HotelPlatform.Service
{
    public interface ISysDepartmentService
    {
        void SyncDepartments();
        IEnumerable<SysDepartment> GetDepartments(int[] deptIds = null);
        IEnumerable<SysDepartmentResponse> GetSysDepartments(SysDepartmentQueryRequest queryRequest);
    }
}
