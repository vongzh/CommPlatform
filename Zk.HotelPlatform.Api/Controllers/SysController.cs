using System;
using System.Collections.Generic;
using System.Web.Http;
using Zk.HotelPlatform.Api.Filters;
using Zk.HotelPlatform.Model;
using Zk.HotelPlatform.Model.Basic.Pager;
using Zk.HotelPlatform.Model.Request;
using Zk.HotelPlatform.Model.Request.Sys;
using Zk.HotelPlatform.Model.Response;
using Zk.HotelPlatform.Model.Response.Log;
using Zk.HotelPlatform.Service;
using Zk.HotelPlatform.Utils;
using Zk.HotelPlatform.Utils.Global;

namespace Zk.HotelPlatform.Api.Controllers
{
    /// <summary>
    /// 系统
    /// </summary>
    [Authorize(Roles = "Client")]
    [SysAuthorize]
    [ResponseHandler]
    public class SysController : BaseController
    {
        private readonly ISysMenuService _sysMenuService = null;
        private readonly ISysUserService _sysUserService = null;
        private readonly ISysRoleService _sysRoleService = null;
        private readonly ISysModuleService _sysModuleService = null;
        private readonly ISysUserRoleService _sysUserRoleService = null;
        private readonly ISysUserGroupService _sysUserGroupService = null;
        private readonly ISysUserGroupUserService _sysUserGroupUserService = null;
        private readonly ISysRolePermissionService _sysRolePermissionService = null;
        private readonly ISysUserGroupModuleService _sysUserGroupModuleService = null;
        private readonly ILoginLogService _loginLogService = null;
        private readonly ISysConfigService _sysConfigService = null;
        private readonly ISysDepartmentService _sysDepartmentService = null;
        private readonly IOperationLogService _operationLogService = null;
     

        public SysController(ISysDepartmentService sysDepartmentService, IOperationLogService operationLogService, ISysConfigService sysConfigService, ISysUserRoleService sysUserRoleService, ILoginLogService loginLogService, ISysUserGroupModuleService sysUserGroupModuleService, ISysModuleService sysModuleService, ISysUserGroupUserService sysUserGroupUserService, ISysUserGroupService sysUserGroupService, ISysMenuService sysMenuService, ISysUserService sysUserService, ISysRoleService sysRoleService, ISysRolePermissionService sysRolePermissionService
              

            )
        {
            this._sysMenuService = sysMenuService;
            this._sysUserService = sysUserService;
            this._sysRoleService = sysRoleService;
            this._sysModuleService = sysModuleService;
            this._sysUserRoleService = sysUserRoleService;
            this._sysUserGroupService = sysUserGroupService;
            this._sysUserGroupUserService = sysUserGroupUserService;
            this._sysRolePermissionService = sysRolePermissionService;
            this._sysUserGroupModuleService = sysUserGroupModuleService;
            this._loginLogService = loginLogService;
            this._sysConfigService = sysConfigService;
            this._operationLogService = operationLogService;
            this._sysDepartmentService = sysDepartmentService;
          
        }

        #region 部门
        /// <summary>
        /// 查询部门
        /// </summary>
        /// <param name="queryRequest"></param>
        /// <returns></returns>
        
        [HttpPost]
        [Route("SysDepartment/Query")]
        public IEnumerable<SysDepartmentResponse> GetDepartments(SysDepartmentQueryRequest queryRequest)
        {
            if (queryRequest == null)
                queryRequest = new SysDepartmentQueryRequest();

            return this._sysDepartmentService.GetSysDepartments(queryRequest);
        }

        /// <summary>
        /// 同步部门
        /// </summary>
        
        [HttpPost]
        [Route("SysDepartment/Sync")]
        public void SyncDepartments()
        {
            this._sysDepartmentService.SyncDepartments();
        }

        /// <summary>
        /// 同步部门员工
        /// </summary>
        
        [HttpPost]
        [Route("SysDepartment/SyncEmployees")]
        public void SyncEmployees([FromUri] int? departmentId)
        {
            int[] departmentIds = null;
            if (departmentId.HasValue)
            {
                if (departmentId.Value > 0)
                {
                    departmentIds = new int[] { departmentId.Value };
                }
            }
            this._sysUserService.SyncEmployees(departmentIds);
        }
        #endregion

        #region 菜单
        /// <summary>
        /// 查询系统菜单
        /// </summary>
        /// <param name="queryRequest"></param>
        /// <returns></returns>
        
        [HttpPost]
        [Route("SysMenu/Query")]
        public IEnumerable<SysMenuResponse> GetMenus(SysMenuQueryRequest queryRequest)
        {
            if (queryRequest == null)
                queryRequest = new SysMenuQueryRequest();

            return this._sysMenuService.GetMenus(queryRequest);
        }

        /// <summary>
        /// 查询系统菜单
        /// </summary>
        /// <returns></returns>
        
        [HttpGet]
        [Route("SysMenu/QueryNest")]
        public IEnumerable<SysMenuResponse> GetMenus()
        {
            return this._sysMenuService.GetNestMenus();
        }

        /// <summary>
        /// 删除菜单
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        
        [HttpGet]

        [Route("SysMenu/Delete")]
        public bool DeleteMenu(int id)
        {
            return this._sysMenuService.Delete(id, base.CurrentSysUser.UserId);
        }

        /// <summary>
        /// 启用/禁用
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        
        [HttpGet]

        [Route("SysMenu/Enabled")]
        public bool EnabledMenu(int id)
        {
            return this._sysMenuService.Enabled(id, base.CurrentSysUser.UserId);
        }

        /// <summary>
        /// 查询系统菜单
        /// </summary>
        /// <param name="menuId"></param>
        /// <returns></returns>
        
        [HttpGet]
        [Route("SysMenu/Get")]
        public SysMenuResponse GetMenu(int menuId)
        {
            return this._sysMenuService.GetMenu(menuId);
        }

        /// <summary>
        /// 新增系统菜单
        /// </summary>
        /// <param name="addRequest"></param>
        /// <returns></returns>
        
        [HttpPost]

        [Route("SysMenu/Save")]
        public bool SaveMenu(SysMenuAddRequest addRequest)
        {
            if (addRequest == null)
                throw new ArgumentNullException("菜单");

            if (string.IsNullOrWhiteSpace(addRequest.Name))
                throw new ArgumentNullException("菜单名称");

            return this._sysMenuService.SaveMenu(addRequest, base.CurrentSysUser.UserId);
        }
        #endregion

        #region 角色
        /// <summary>
        /// 查询用户角色
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        
        [HttpGet]
        [Route("SysRole/GetUserRole")]
        public IEnumerable<SysUserRole> GetUserRoles(int userId)
        {
            var sysUser = _sysUserService.GetUserInfoByUserId(userId);
            if (sysUser == null)
                throw new BusinessException("未找到用户信息");

            return this._sysUserRoleService.GetUserRoles(sysUser.Id);
        }

        /// <summary>
        /// 保存用户角色
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="roles"></param>
        /// <returns></returns>
        
        [HttpPost]

        [Route("SysRole/SaveUserRole")]
        public bool SaveUserRoles([FromUri] int userId, [FromBody] IEnumerable<int> roles)
        {
            return this._sysUserRoleService.SaveUserRoles(userId, roles, base.CurrentSysUser.UserId);
        }

        /// <summary>
        /// 查询系统角色
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        
        [HttpGet]
        [Route("SysRole/Get")]
        public SysRoleResponse GetRole(int id)
        {
            return this._sysRoleService.GetRole(id);
        }

        /// <summary>
        /// 查询系统角色
        /// </summary>
        /// <param name="queryRequest"></param>
        /// <returns></returns>
        
        [HttpPost]
        [Route("SysRole/Query")]
        public IEnumerable<SysRoleResponse> GetRoles(SysRoleQueryRequest queryRequest)
        {
            if (queryRequest == null)
                queryRequest = new SysRoleQueryRequest();

            return this._sysRoleService.GetRoles(queryRequest);
        }

        /// <summary>
        /// 查询系统角色
        /// </summary>
        /// <returns></returns>
        
        [HttpGet]
        [Route("SysRole/GetAll")]
        public IEnumerable<SysRoleResponse> GetRoles()
        {
            return this._sysRoleService.GetRoles();
        }

        /// <summary>
        /// 新增角色
        /// </summary>
        /// <param name="addRequest"></param>
        /// <returns></returns>
        
        [HttpPost]

        [Route("SysRole/Save")]
        public bool SaveRole(SysRoleAddRequest addRequest)
        {
            if (addRequest == null)
                throw new ArgumentNullException("角色");

            if (string.IsNullOrWhiteSpace(addRequest.RoleName))
                throw new ArgumentNullException("角色名称");
            if (string.IsNullOrWhiteSpace(addRequest.Meta))
                throw new ArgumentNullException("标记");

            return this._sysRoleService.SaveRole(addRequest, base.CurrentSysUser.UserId);
        }

        /// <summary>
        /// 删除角色
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        
        [HttpGet]

        [Route("SysRole/Delete")]
        public bool DeleteRole(int id)
        {
            return this._sysRoleService.Delete(id, base.CurrentSysUser.UserId);
        }
        #endregion

        #region 角色权限
        /// <summary>
        /// 获取角色权限
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        
        [HttpGet]
        [Route("SysRolePermission/Get")]
        public IEnumerable<SysRolePermission> GetRolePermissions(int roleId)
        {
            return _sysRolePermissionService.GetRolePermissions(roleId);
        }

        /// <summary>
        /// 保存角色权限
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="permissionIds"></param>
        /// <returns></returns>
        
        [HttpPost]
        [Route("SysRolePermission/Save")]
        public bool SaveRolePermission([FromUri] int roleId, [FromBody] IEnumerable<int> permissionIds)
        {
            return _sysRolePermissionService.SaveRolePermission(roleId, permissionIds, base.CurrentSysUser.UserId);
        }
        #endregion

        #region 用户组
        /// <summary>
        /// 查询用户组
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        
        [HttpGet]
        [Route("SysUserGroup/Get")]
        public SysUserGroupResponse GetUserGroup(int id)
        {
            return this._sysUserGroupService.GetUserGroup(id);
        }

        /// <summary>
        /// 查询用户组
        /// </summary>
        /// <returns></returns>
        
        [HttpGet]
        [Route("SysUserGroup/GetAll")]
        public IEnumerable<SysUserGroupResponse> GetUserGroups()
        {
            return this._sysUserGroupService.GetUserGroups();
        }

        /// <summary>
        /// 查询用户组
        /// </summary>
        /// <param name="queryRequest"></param>
        /// <returns></returns>
        
        [HttpPost]
        [Route("SysUserGroup/Query")]
        public IEnumerable<SysUserGroupResponse> GetUserGroups(SysUserGroupQueryRequest queryRequest)
        {
            if (queryRequest == null)
                queryRequest = new SysUserGroupQueryRequest();

            return this._sysUserGroupService.GetUserGroups(queryRequest);
        }

        /// <summary>
        /// 新增用户组
        /// </summary>
        /// <param name="addRequest"></param>
        /// <returns></returns>
        
        [HttpPost]

        [Route("SysUserGroup/Save")]
        public bool SaveUserGroup(SysUserGroupAddRequest addRequest)
        {
            if (addRequest == null)
                throw new ArgumentNullException("角色");

            if (string.IsNullOrWhiteSpace(addRequest.GroupName))
                throw new ArgumentNullException("组名称");
            if (string.IsNullOrWhiteSpace(addRequest.Meta))
                throw new ArgumentNullException("标识");

            return this._sysUserGroupService.SaveUserGroup(addRequest, base.CurrentSysUser.UserId);
        }

        /// <summary>
        /// 删除用户组
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        
        [HttpGet]

        [Route("SysUserGroup/Delete")]
        public bool DeleteUserGroup(int id)
        {
            return this._sysUserGroupService.Delete(id, base.CurrentSysUser.UserId);
        }
        #endregion

        #region 用户组用户
        /// <summary>
        /// 保存用户用户组
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="groupIds"></param>
        /// <returns></returns>
        
        [HttpPost]

        [Route("SysUserGroupUser/SaveUserGroups")]
        public bool SaveUserGroups([FromUri] int userId, [FromBody] IEnumerable<int> groupIds)
        {
            return this._sysUserGroupUserService.SaveUserGroups(userId, groupIds, base.CurrentSysUser.UserId);
        }

        /// <summary>
        /// 查询用户用户组
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        
        [HttpGet]
        [Route("SysUserGroupUser/GetUserGroups")]
        public IEnumerable<SysUserGroupUser> GetUserGroups(int userId)
        {
            var sysUser = _sysUserService.GetUserInfoByUserId(userId);
            if (sysUser == null)
                throw new BusinessException("未找到用户信息");
            return this._sysUserGroupUserService.GetUserGroups(sysUser.Id);
        }

        /// <summary>
        /// 保存用户组用户
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="sysUserGroupUsers"></param>
        /// <returns></returns>
        
        [HttpPost]

        [Route("SysUserGroupUser/Save")]
        public bool SaveSysUserGroupUsers([FromUri] int groupId, [FromBody] IEnumerable<int> sysUserGroupUsers)
        {
            return this._sysUserGroupUserService.SaveSysUserGroupUsers(groupId, sysUserGroupUsers, base.CurrentSysUser.UserId);
        }

        /// <summary>
        /// 查询用户组用户
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        
        [HttpGet]
        [Route("SysUserGroupUser/Get")]
        public IEnumerable<SysUserGroupUser> GetSysUserGroupUsers(int groupId)
        {
            return this._sysUserGroupUserService.GetSysUserGroupUsers(groupId);
        }
        #endregion

        #region 模块
        /// <summary>
        /// 查询系统模块
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        
        [HttpGet]
        [Route("SysModule/Get")]
        public SysModuleResponse GetModule(int id)
        {
            return this._sysModuleService.GetModule(id);
        }

        /// <summary>
        /// 查询系统模块
        /// </summary>
        /// <param name="queryRequest"></param>
        /// <returns></returns>
        
        [HttpPost]
        [Route("SysModule/Query")]
        public IEnumerable<SysModuleResponse> GetModules(SysModuleQueryRequest queryRequest)
        {
            if (queryRequest == null)
                queryRequest = new SysModuleQueryRequest();

            return this._sysModuleService.GetSysModules(queryRequest);
        }

        /// <summary>
        /// 新增模块
        /// </summary>
        /// <param name="addRequest"></param>
        /// <returns></returns>
        
        [HttpPost]

        [Route("SysModule/Save")]
        public bool SaveModule(SysModuleAddRequest addRequest)
        {
            if (addRequest == null)
                throw new ArgumentNullException("模块");

            if (string.IsNullOrWhiteSpace(addRequest.ModuleName))
                throw new ArgumentNullException("模块名称");
            if (string.IsNullOrWhiteSpace(addRequest.Meta))
                throw new ArgumentNullException("标记");

            return this._sysModuleService.SaveModule(addRequest, base.CurrentSysUser.UserId);
        }

        /// <summary>
        /// 删除模块
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        
        [HttpGet]

        [Route("SysModule/Delete")]
        public bool DeleteModule(int id)
        {
            return this._sysModuleService.Delete(id, base.CurrentSysUser.UserId);
        }
        #endregion

        #region 用户组模块
        /// <summary>
        /// 保存用户组模块
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="sysUserGroupModules"></param>
        /// <returns></returns>
        
        [HttpPost]

        [Route("SysUserGroupModule/Save")]
        public bool SaveUserGroupModules([FromUri] int groupId, [FromBody] IEnumerable<int> sysUserGroupModules)
        {
            return this._sysUserGroupModuleService.SaveUserGroupModule(groupId, sysUserGroupModules, base.CurrentSysUser.UserId);
        }

        /// <summary>
        /// 查询用户组模块
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        
        [HttpGet]
        [Route("SysUserGroupModule/Get")]
        public IEnumerable<SysUserGroupModule> GetUserGroupModules(int groupId)
        {
            return this._sysUserGroupModuleService.GetUserGroupModules(groupId);
        }
        #endregion

        #region 用户
        /// <summary>
        /// 获取用户信息(也适用于子用户登录)
        /// </summary>
        /// <returns></returns>
        
        [HttpPost]
        [Route("SysUser/Get")]
        public SysUserResponse GetUserInfo()
        {
            return this._sysUserService.GetUserInfo(CurrentSysUser.UserId, true);
        }

        /// <summary>
        /// 查询用户信息(也适用于客户点击查询子用户)
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        
        [HttpPost]
        [Route("SysUser/Get")]
        public SysUserResponse GetUserInfoByUserId(int userId)
        {
            return this._sysUserService.GetUserInfo(userId, false);
        }
        /// <summary>
        /// 获取用户列表
        /// </summary>
        /// <returns></returns>
        
        [HttpGet]
        [Route("SysUser/GetList")]
        public IEnumerable<SysUserResponse> GetUserInfos()
        {
            return _sysUserService.GetUserInfos();
        }

        /// <summary>
        /// 根据部门查询用户
        /// </summary>
        /// <returns></returns>
        
        [HttpGet]
        [Route("SysUser/GetUsersByDeptId")]
        public IEnumerable<SysUserResponse> GetUsersByDeptId([FromUri] int deptId)
        {
            return _sysUserService.GetUsersByDeptId(new int[] { deptId });
        }

        /// <summary>
        /// 查询用户信息
        /// </summary>
        /// <param name="queryRequest"></param>
        /// <returns></returns>
        
        [HttpPost]
        [Route("SysUser/Query")]
        public PageResult<SysUserResponse> GetUserInfos(SysUserQueryRequest queryRequest)
        {
            if (!queryRequest.UserType.HasValue)
                throw new BusinessException("参数不能为空");
            return this._sysUserService.QueryUserInfos(queryRequest);
        }

        /// <summary>
        /// 保存用户/子用户
        /// </summary>
        /// <param name="addRequest"></param>
        /// <returns></returns>
        
        [HttpPost]
        [Route("SysUser/Save")]
        public bool SaveUser(SysUserAddRequest addRequest)
        {
            if (addRequest == null)
            {
                throw new ArgumentNullException("参数不能为空");
            }
            //if (addRequest.Id <= 0)
            //{
            //    throw new ArgumentNullException("用户Id不能为空");
            //}
            if (string.IsNullOrWhiteSpace(addRequest.UserName))
            {
                throw new ArgumentNullException("用户名不能为空");
            }
            if (string.IsNullOrWhiteSpace(addRequest.NickName))
            {
                throw new ArgumentNullException("姓名不能为空");
            }
            if (string.IsNullOrWhiteSpace(addRequest.Mail))
            {
                throw new BusinessException("邮箱地址不能为空");
            }
            if (!string.IsNullOrWhiteSpace(addRequest.Mail))
            {
                if (!RegexUtil.MailRegexMatch(addRequest.Mail.Trim()))
                {
                    throw new BusinessException("邮箱格式不正确");
                }
            }
            if (!string.IsNullOrWhiteSpace(addRequest.Mobile))
            {
                if (!RegexUtil.PhoneRegexMatch(addRequest.Mobile.Trim()))
                {
                    throw new BusinessException("手机号格式不正确");
                }
            }

            return _sysUserService.SaveUser(addRequest, base.CurrentSysUser.UserId, out string addedUserId);
        }

        /// <summary>
        /// 添加用户/子用户
        /// </summary>
        /// <param name="addRequest"></param>
        /// <returns></returns>
        
        [HttpPost]
        [Route("SysUser/Add")]
        public string AddSysUser(SysUserAddRequest addRequest)
        {
            if (addRequest == null)
            {
                throw new ArgumentNullException("参数不能为空");
            }
            if (addRequest.IsSubUser == (int)GlobalEnum.YESOrNO.N)
            {
                //主账号时校验 子账号时跟主账号保持一致
                if (addRequest.UserType <= 0)
                {
                    throw new BusinessException("用户类型不正确");
                }
                if (addRequest.UserPlatformId <= 0)
                {
                    throw new BusinessException("用户所属平台不正确");
                }
            }
            else
            {
                if (addRequest.ParentUserId <= 0)
                {
                    throw new BusinessException("主账号Id为空");
                }
                if (addRequest.ParentUserId != base.CurrentSysUser.UserId)
                {
                    throw new BusinessException("登录用户和主账号不一致");
                }
                if (addRequest.UserType > 0)
                {
                    throw new BusinessException("用户类型参数无效");
                }
                if (addRequest.UserPlatformId > 0)
                {
                    throw new BusinessException("用户所属平台参数无效");
                }
            }
            if (string.IsNullOrWhiteSpace(addRequest.UserName))
            {
                throw new ArgumentNullException("用户名不能为空");
            }
            if (string.IsNullOrWhiteSpace(addRequest.NickName))
            {
                throw new ArgumentNullException("姓名不能为空");
            }
            if (!string.IsNullOrWhiteSpace(addRequest.Mail))
            {
                if (!RegexUtil.MailRegexMatch(addRequest.Mail.Trim()))
                {
                    throw new BusinessException("邮箱格式不正确");
                }
            }
            if (!string.IsNullOrWhiteSpace(addRequest.Mobile))
            {
                if (!RegexUtil.PhoneRegexMatch(addRequest.Mobile.Trim()))
                {
                    throw new BusinessException("手机号格式不正确");
                }
            }
            if (!_sysUserService.SaveUser(addRequest, base.CurrentSysUser.UserId, out string addedUserId))
            {
                throw new BusinessException("用户保存失败");
            }
            return addedUserId;
        }

        /// <summary>
        /// 获取子用户列表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        
        [HttpPost]
        [Route("SysUser/GetSubUserInfos")]
        public PageResult<SysUserResponse> GetSubUserInfos(SysSubUserQueryRequest request)
        {
            request.ParentUserId = base.CurrentSysUser?.UserId;
            return _sysUserService.QuerySubUserInfos(request);
        }
        /// <summary>
        /// 重置密码
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        
        [HttpPost]

        [Route("SysUser/ResetPwd")]
        public bool ResetPwd(int userId)
        {
            if (base.CurrentSysUser.UserType != (int)GlobalEnum.UserType.BACKSTAGE)
                throw new BusinessException("无法操作");

            return _sysUserService.ResetPwd(userId, base.CurrentSysUser.UserId);
        }


        /// <summary>
        /// 检查手机号是否存在
        /// </summary>
        /// <param name="mobile"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        
        [HttpPost]
        [Route("SysUser/CheckSysUserMobileIsExist")]
        public bool CheckSysUserMobileIsExist([FromUri] int userId, [FromUri] string mobile)
        {
            if (!RegexUtil.PhoneRegexMatch(mobile))
            {
                throw new BusinessException("请输入正确的手机号");
            }
            return _sysUserService.CheckSysUserMobileIsExist(userId, mobile);
        }

        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        
        [HttpGet]

        [Route("SysUser/Delete")]
        public bool DeleteUser(int userId)
        {
            return _sysUserService.Delete(userId, base.CurrentSysUser.UserId);
        }

        /// <summary>
        /// 禁用/启用
        /// </summary>
        /// <param name="disabled"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        
        [HttpGet]
        [Route("SysUser/Disabled")]
        public bool Disabled(int disabled, int userId)
        {
            return _sysUserService.Disabled(disabled, userId, base.CurrentSysUser.UserId);
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="pwd"></param>
        /// <param name="oldPwd"></param>
        /// <returns></returns>
        
        [HttpPost]
        [Route("SysUser/ChangePwd")]
        public bool ChangePwd([FromUri] string pwd, [FromUri] string oldPwd)
        {
            return _sysUserService.ChangePwd(pwd, oldPwd, base.CurrentSysUser.UserId);
        }

        /// <summary>
        /// 系统用户登录
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="pwd"></param>
        /// <param name="seed"></param>
        /// <returns></returns>
        
        [HttpGet]
        [AllowAnonymous]
        [Route("SysUser/Login")]
        public string Login(string userName, string pwd, string seed)
        {
            return this._sysUserService.Login(userName, pwd, seed, base.Request.Headers.UserAgent.ToString(), base.ClientIP);
        }

        /// <summary>
        /// 系统用户微信登录
        /// </summary>
        /// <param name="code"></param>
        /// <param name="state"></param>
        /// <param name="seed"></param>
        /// <returns></returns>
        
        [HttpGet]
        [AllowAnonymous]
        [Route("SysUser/WechatLogin")]
        public string WechatLogin(string code, string state, string seed)
        {
            if (string.IsNullOrWhiteSpace(state))
            {
                throw new BusinessException("state参数错误");
            }
            return this._sysUserService.WechatLogin(code, seed, base.Request.Headers.UserAgent.ToString(), base.ClientIP);
        }


        /// <summary>
        /// 登出
        /// </summary>
        /// <returns></returns>
        
        [HttpGet]
        [Route("SysUser/Logout")]
        public bool Logout()
        {
            return this._sysUserService.Logout(base.CurrentSysUser, base.Request.Headers.UserAgent.ToString(), base.ClientIP);
        }
        #endregion

        #region 日志
        /// <summary>
        /// 系统日志
        /// </summary>
        /// <param name="queryRequest"></param>
        /// <returns></returns>
        
        [HttpPost]
        [Route("Log/LoginLog")]
        public PageResult<LoginLog> GetLoginLogs(LogQueryRequest queryRequest)
        {
            if (queryRequest == null)
            {
                queryRequest = new LogQueryRequest();
            }

            return _loginLogService.GetLoginLogs(queryRequest);
        }

        /// <summary>
        /// 操作日志
        /// </summary>
        /// <param name="queryRequest"></param>
        /// <returns></returns>
        
        [HttpPost]
        [Route("Log/OperationLog")]
        public PageResult<OperationLogResponse> GetOperationLogs(LogQueryRequest queryRequest)
        {
            if (queryRequest == null)
            {
                queryRequest = new LogQueryRequest();
            }

            return _operationLogService.GetOperationLogs(queryRequest);
        }

        /// <summary>
        /// 操作日志
        /// </summary>
        /// <param name="log"></param>
        /// <returns></returns>
        
        [HttpPost]
        [Route("Log/AddOperationLog")]
        public bool AddOperationLog([FromBody] (string OptType, string BusinessParam) log)
        {
            return _operationLogService.AddOperationLog(log.OptType, log.BusinessParam, base.CurrentSysUser);
        }

        /// <summary>
        /// 操作日志时间线
        /// </summary>
        /// <param name="queryRequest"></param>
        /// <returns></returns>
        
        [HttpPost]
        [Route("Log/OperationLogTime")]
        public PageResult<OperationLogResponse> GetOperationLogTimes(LogQueryRequest queryRequest)
        {
            if (queryRequest == null)
            {
                queryRequest = new LogQueryRequest();
            }

            return _operationLogService.GetOperationLogs(queryRequest, base.CurrentSysUser.UserId);
        }
        #endregion

        #region 系统配置
        /// <summary>
        /// 数据配置
        /// </summary>
        /// <param name="queryRequest"></param>
        /// <returns></returns>
        
        [HttpPost]
        [Route("SysConfig/GetDataConfigs")]
        public PageResult<SysConfig> GetDataConfigs(SysConfigQueryRequest queryRequest)
        {
            return _sysConfigService.GetDataConfigs(queryRequest);
        }

        /// <summary>
        /// 保存数据配置
        /// </summary>
        /// <param name="sysConfig"></param>
        /// <returns></returns>
        
        [HttpPost]

        [Route("SysConfig/DataConfig")]
        public bool DataConfig(SysConfig sysConfig)
        {
            if (string.IsNullOrWhiteSpace(sysConfig.Domain) || string.IsNullOrWhiteSpace(sysConfig.Key) || string.IsNullOrWhiteSpace(sysConfig.Value))
            {
                throw new BusinessException("请完整填写必填项");
            }

            return _sysConfigService.SaveDataConfig(sysConfig, base.CurrentSysUser.UserId);
        }

        /// <summary>
        /// 删除系统配置
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        
        [HttpPost]
        [Route("SysConfig/Delete")]
        public bool DeleteSysConfig(int id)
        {
            return _sysConfigService.Delete(id, base.CurrentSysUser.UserId);
        }
        #endregion

    }
}
