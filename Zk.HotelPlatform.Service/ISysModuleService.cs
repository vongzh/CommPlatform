using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zk.HotelPlatform.Model;
using Zk.HotelPlatform.Model.Basic.Pager;
using Zk.HotelPlatform.Model.Request;
using Zk.HotelPlatform.Model.Response;

namespace Zk.HotelPlatform.Service
{
    public interface ISysModuleService : IDataService<SysModule>
    {
        IEnumerable<SysModule> GetSysModules();
        IEnumerable<SysModuleResponse> GetSysModules(IEnumerable<int> moduleIds);
        SysModuleResponse GetModule(int id);
        IEnumerable<SysModuleResponse> GetSysModules(SysModuleQueryRequest queryRequest);
        bool SaveModule(SysModuleAddRequest addRequest, int userId);
        bool Delete(int id, int userId);
    }
}
