using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zk.HotelPlatform.Model;
using Zk.HotelPlatform.Model.Basic.Pager;
using Zk.HotelPlatform.Model.Request;

namespace Zk.HotelPlatform.Service
{
    public interface ISysConfigService
    {
       
        int GetCancelEarlyTime();
        string GetSysUserDefaultPwd();

      
        bool Delete(int id, int userId);
        int GetLoginErrorLimitCount();
       
        PageResult<SysConfig> GetDataConfigs(SysConfigQueryRequest queryRequest);
       
        bool SaveDataConfig(SysConfig sysConfig, int userId);
        T GetConfig<T>(string domain, string keyName);
        bool GetAllowRegister();
        int GetMailSendLimitCount();
    }
}
