using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Zk.HotelPlatform.CacheModel;
using Zk.HotelPlatform.Service;
using Zk.HotelPlatform.Utils.Global;

namespace Zk.HotelPlatform.Api.Filters
{
    public class PermissionAttribute : ActionFilterAttribute
    {
        private const string _sysUserSession = GlobalConst.SYSUSER_SESSION_CONTEXT;
        private const string _reasonPhrase = "Permission";

        //private readonly ISysModuleService _sysModuleService = null;
        //private readonly ISysUserGroupUserService _sysUserGroupUserService = null;
        //private readonly ISysUserGroupModuleService _sysUserGroupModuleService = null;

        //public PermissionAttribute(ISysModuleService sysModuleService, ISysUserGroupModuleService sysUserGroupModuleService, ISysUserGroupUserService sysUserGroupUserService)
        //{
        //    _sysModuleService = sysModuleService;
        //    _sysUserGroupUserService = sysUserGroupUserService;
        //    _sysUserGroupModuleService = sysUserGroupModuleService;
        //}

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            var sysModuleService = (ISysModuleService)actionContext.RequestContext.Configuration.DependencyResolver.GetService(typeof(ISysModuleService));
            var sysModules = sysModuleService.GetSysModules().Where(x => x.Type == (int)GlobalEnum.ModuleType.API 
            && x.Control == (int)GlobalEnum.YESOrNO.Y 
            && !string.IsNullOrWhiteSpace(x.Path));

            if (sysModules == null)
                return;

            var path = actionContext.Request.RequestUri.LocalPath;

            var moudle = sysModules.FirstOrDefault(x => x.Path.TrimStart('/').TrimEnd('/').ToLower() == path.TrimStart('/').TrimEnd('/').ToLower());
            if (moudle == null)
                return;

            if (!OwinRequestScopeContext.Current.Items.ContainsKey(_sysUserSession))
            {
                actionContext.Response = actionContext.Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "No Permission");
                actionContext.Response.Headers.Add("Access-Control-Expose-Headers", "*");
                actionContext.Response.ReasonPhrase = _reasonPhrase;
                return;
            }

            var sysUserGroupUserService = (ISysUserGroupUserService)actionContext.RequestContext.Configuration.DependencyResolver.GetService(typeof(ISysUserGroupUserService));

            var sysUserSession = (SysUserSession)OwinRequestScopeContext.Current.Items[_sysUserSession];
            var userGroups = sysUserGroupUserService.GetUserGroups(sysUserSession.Id);
            if (userGroups == null || userGroups.Count() == 0)
            {
                actionContext.Response = actionContext.Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "No Permission");
                actionContext.Response.Headers.Add("Access-Control-Expose-Headers", "*");
                actionContext.Response.ReasonPhrase = _reasonPhrase;
                return;
            }


            var sysUserGroupModuleService = (ISysUserGroupModuleService)actionContext.RequestContext.Configuration.DependencyResolver.GetService(typeof(ISysUserGroupModuleService));

            var groupIds = userGroups.Select(x => x.UserGroupId);
            var groupModules = sysUserGroupModuleService.GetUserGroupModules(groupIds);
            if (groupModules == null || groupModules.Count() == 0)
            {
                actionContext.Response = actionContext.Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "No Permission");
                actionContext.Response.Headers.Add("Access-Control-Expose-Headers", "*");
                actionContext.Response.ReasonPhrase = _reasonPhrase;
                return;
            }

            if (!groupModules.Any(x => x.ModuleId == moudle.Id))
            {
                actionContext.Response = actionContext.Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "No Permission");
                actionContext.Response.Headers.Add("Access-Control-Expose-Headers", "*");
                actionContext.Response.ReasonPhrase = _reasonPhrase;
                return;
            }

            base.OnActionExecuting(actionContext);
        }
    }
}
