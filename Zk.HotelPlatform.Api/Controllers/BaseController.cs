using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Zk.HotelPlatform.Utils;
using Zk.HotelPlatform.CacheModel;
using Zk.HotelPlatform.Utils.Global;

namespace Zk.HotelPlatform.Api.Controllers
{
    public class BaseController : ApiController
    {
        private const string _sysUserSession = GlobalConst.SYSUSER_SESSION_CONTEXT;

        /// <summary>
        /// 当前登录用户
        /// </summary>
        public SysUserSession CurrentSysUser
        {
            get
            {
                try
                {
                    if (!OwinRequestScopeContext.Current.Items.ContainsKey(_sysUserSession))
                        throw new BusinessException("用户信息获取失败");
                    return (SysUserSession)OwinRequestScopeContext.Current.Items[_sysUserSession];
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        /// <summary>
        /// 客户端IP
        /// </summary>
        public string ClientIP
        {
            get
            {
                return Request.GetClientIp();
            }
        }
    }
}
