using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zk.HotelPlatform.CacheModel;
using Zk.HotelPlatform.Model;
using Zk.HotelPlatform.Model.Request;
using Zk.HotelPlatform.Model.Response;
using Zk.HotelPlatform.Model.Basic.Pager;
using Zk.HotelPlatform.Utils.Global;
using Zk.HotelPlatform.Model.Request.Sys;

namespace Zk.HotelPlatform.Service
{
    public interface ISysUserService : IDataService<SysUser>
    {
        SysUser GetApiUser(string userName, string pwd);
        bool ResetPwd(int userId, int optUserId);
        string Login(string userName, string pwd, string seed, string userAgent, string clientIP);
        string WechatLogin(string code, string seed, string userAgent, string clientIP);
        bool Logout(SysUserSession sysUserSession, string userAgent, string clientIP);
        SysUserResponse GetUserInfo(int userId, bool includePower = false);
        IEnumerable<SysUserResponse> GetUserInfos();
        IEnumerable<SysUser> GetUserInfos_Origin();
        bool Delete(int userId, int optUserId);
        bool SaveUser(SysUserAddRequest addRequest, int userId, out string addedUserId);

        PageResult<SysUserResponse> QueryUserInfos(SysUserQueryRequest queryRequest);
        PageResult<SysUserResponse> QuerySubUserInfos(SysSubUserQueryRequest queryRequest);
        SysUserSession GetUserSessionCache(string token);
        bool CheckUserNameIsExist(string userName);
        bool CheckUserMobileIsExist(string mobile);
        bool CheckSysUserMobileIsExist(int userId, string mobile);
        bool SetCustomer(int userId, string customerNo, string nickName);
        bool ChangePwd(string pwd, string oldPwd, int userId);
        string GetUserTokenCache(string userId);
        bool FindPwd(string userName, string clientIP, string randstr, string ticket, int ret);
        string GetUserMobile(string userName);
        bool ResetPwd(string mobile, string pwd, string requestId);
        SysUser GetSysUser(string uname, GlobalEnum.UserType userType = GlobalEnum.UserType.ALL);
        string GetCustomerNo(string uname);
        SysUser GetSysUserByMobile(string mobile);
        bool ApiLogin(string userName, string pwd, string clientIP);
        SysUser GetUserInfoByUserId(int userId);
        bool Disabled(int disabled, int userId, int optUserId);
        IEnumerable<int> GetUserIdsByParentId(int parentUserId);
        IEnumerable<SysUserResponse> GetUsersByDeptId(int[] deptIds);
        void SyncEmployees(int[] deptIds = null);
    }
}
