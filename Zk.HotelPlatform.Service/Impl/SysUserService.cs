using AutoMapper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Zk.HotelPlatform.CacheModel;
using Zk.HotelPlatform.CacheProvider;
using Zk.HotelPlatform.Model;
using Zk.HotelPlatform.Model.Request;
using Zk.HotelPlatform.Model.Response;
using Zk.HotelPlatform.Utils;
using Zk.HotelPlatform.Utils.Global;
using Zk.HotelPlatform.Model.Basic.Pager;
using Zk.HotelPlatform.DBProvider;
using Senparc.Weixin.Work.Containers;
using Senparc.Weixin.Work.AdvancedAPIs;
using Zk.HotelPlatform.Model.Request.Sys;
using Zk.HotelPlatform.Utils.Log;

namespace Zk.HotelPlatform.Service.Impl
{
    public class SysUserService : DataService<SysUser>, ISysUserService
    {
        private readonly IMapper _mapper = null;
        private readonly ISysRoleService _sysRoleService;
        private readonly IWeChatService _weChatService;
        private readonly ISysMenuService _sysMenuService;
        private readonly ISysModuleService _sysModuleService;
        private readonly ISysUserRoleService _sysUserRoleService;
        private readonly ISysUserGroupUserService _sysUserGroupUserService;
        private readonly ISysRolePermissionService _sysRolePermissionService;
        private readonly ISysUserGroupModuleService _sysUserGroupModuleService;
        private readonly ICaptchaService _captchaService;
        private readonly ILoginLogService _loginLogService;
        private readonly ISysConfigService _sysConfigService;
        private readonly ISysUserGroupService _sysUserGroupService;
        private readonly ISysDepartmentService _sysDepartmentService;
        private readonly IOperationLogService _operationLogService;

        private readonly ISysUserCacheProvider _sysUserCacheProvider;

        private const string _sequenceName = "Seq_UserNo";

        private Dictionary<int, SysUser> _systemUsers = new Dictionary<int, SysUser>()
        {
            { GlobalConst.SYS_PAYMENT_CALLBACK,new SysUser(){
                UserId = GlobalConst.SYS_PAYMENT_CALLBACK,
                NickName = GlobalConst.SYS_PAYMENT_CALLBACK_USERNAME,
                UserType = (int)GlobalEnum.UserType.SYSTEM
            }},
            { GlobalConst.SYS_PURCHASE,new SysUser(){
                UserId = GlobalConst.SYS_PURCHASE,
                NickName = GlobalConst.SYS_PURCHASE_USERNAME,
                UserType = (int)GlobalEnum.UserType.SYSTEM
            }},
            { GlobalConst.SYS_TASK,new SysUser(){
                UserId = GlobalConst.SYS_TASK,
                NickName = GlobalConst.SYS_TASK_USERNAME,
                UserType = (int)GlobalEnum.UserType.SYSTEM
            }},
        };

        public SysUserService(IMapper mapper, IDataProvider<SysUser> dataProvider, ISysDepartmentService sysDepartmentService, IWeChatService weChatService, ISysUserGroupService sysUserGroupService, IOperationLogService operationLogService, ISysConfigService sysConfigService, ILoginLogService loginLogService, ICaptchaService captchaServic,ISysUserGroupModuleService sysUserGroupModuleService, ISysModuleService sysModuleService, ISysUserGroupUserService sysUserGroupUserService, ISysRoleService sysRoleService, ISysMenuService sysMenuService, ISysUserRoleService sysUserService, ISysRolePermissionService sysRolePermissionService, ISysUserCacheProvider sysUserCacheProvider)
                : base(dataProvider)
        {
            this._mapper = mapper;
            this._sysRoleService = sysRoleService;
            this._sysMenuService = sysMenuService;
            this._sysUserRoleService = sysUserService;
            this._sysModuleService = sysModuleService;
            this._captchaService = captchaServic;
            this._sysUserCacheProvider = sysUserCacheProvider;
            this._sysUserGroupUserService = sysUserGroupUserService;
            this._sysRolePermissionService = sysRolePermissionService;
            this._sysUserGroupModuleService = sysUserGroupModuleService;
            this._loginLogService = loginLogService;
            this._sysConfigService = sysConfigService;
            this._sysUserGroupService = sysUserGroupService;
            this._operationLogService = operationLogService;
            this._weChatService = weChatService;
            this._sysDepartmentService = sysDepartmentService;
        }

        /// <summary>
        /// 获取用户列表
        /// </summary>
        /// <returns></returns>
        public IEnumerable<SysUserResponse> GetUserInfos()
        {
            var sysUsers = base.Select(x => x.IsDelete == (int)GlobalEnum.YESOrNO.N);

            return _mapper.Map<IEnumerable<SysUser>, IEnumerable<SysUserResponse>>(sysUsers);
        }

        /// <summary>
        /// 获取用户原始列表
        /// </summary>
        /// <returns></returns>
        public IEnumerable<SysUser> GetUserInfos_Origin()
        {
            var sysUsers = base.Select(x => x.IsDelete == (int)GlobalEnum.YESOrNO.N);

            return sysUsers;
        }


        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="queryRequest"></param>
        /// <returns></returns>
        public PageResult<SysUserResponse> QueryUserInfos(SysUserQueryRequest queryRequest)
        {
            Expression<Func<SysUser, bool>> filter = x => x.UserType == queryRequest.UserType && x.IsDelete == (int)GlobalEnum.YESOrNO.N;
            if (!string.IsNullOrWhiteSpace(queryRequest.UserName))
            {
                filter = filter.And(x => x.UserName.Contains(queryRequest.UserName.Trim()));
            }
            if (!string.IsNullOrWhiteSpace(queryRequest.NickName))
            {
                filter = filter.And(x => x.NickName.Contains(queryRequest.NickName.Trim()));
            }
            if (queryRequest.RegisterTime.BeginTime.HasValue)
            {
                filter = filter.And(x => x.CreateTime >= queryRequest.RegisterTime.BeginTime.Value);
            }
            if (!string.IsNullOrWhiteSpace(queryRequest.Phone))
            {
                filter = filter.And(x => x.Mobile.Contains(queryRequest.Phone));
            }
            if (queryRequest.RegisterTime.EndTime.HasValue)
            {
                var endTime = queryRequest.RegisterTime.EndTime.Value.ToEndDate();
                filter = filter.And(x => x.CreateTime <= endTime);
            }
            if (queryRequest.Disabled.HasValue)
            {
                filter = filter.And(x => x.Disabled == queryRequest.Disabled);
            }
            IOrderedQueryable<SysUser> orderby(IQueryable<SysUser> x) => x.OrderByDescending(z => z.CreateTime);
            var sysUsers = base.Select(out int totalCount, filter, queryRequest.PageIndex, queryRequest.PageSize, orderby);
            if (sysUsers == null || sysUsers.Count() == 0)
                return new PageResult<SysUserResponse>()
                {
                    Rows = new List<SysUserResponse>(),
                    Total = 0
                };
            var sysRoles = _sysRoleService.GetRoles();
            var sysUserResponses = _mapper.Map<IEnumerable<SysUser>, IEnumerable<SysUserResponse>>(sysUsers);
            foreach (var sysUserResponse in sysUserResponses)
            {
                if (sysRoles != null)
                {
                    var sysUserRoles = _sysUserRoleService.GetUserRoles(sysUserResponse.Id);
                    if (sysUserRoles != null && sysUserRoles.Count() > 0)
                    {
                        var sysRoleIds = sysUserRoles.Select(x => x.RoleId);
                        var userRoles = sysRoles.Where(x => sysRoleIds.Contains(x.Id));

                        sysUserResponse.UserRoles = userRoles;
                    }
                }
                sysUserResponse.StatusDesc = GlobalEnum.GetEnumDescription((GlobalEnum.SysUserStatus)sysUserResponse.Status);
            }
            return new PageResult<SysUserResponse>()
            {
                Total = totalCount,
                Rows = sysUserResponses
            };
        }

        /// <summary>
        /// 获取子用户信息
        /// </summary>
        /// <param name="queryRequest"></param>
        /// <returns></returns>
        public PageResult<SysUserResponse> QuerySubUserInfos(SysSubUserQueryRequest queryRequest)
        {
            Expression<Func<SysUser, bool>> filter = x => x.ParentUserId == queryRequest.ParentUserId && x.IsDelete == (int)GlobalEnum.YESOrNO.N;
            if (!string.IsNullOrWhiteSpace(queryRequest.UserName))
            {
                filter = filter.And(x => x.UserName.Contains(queryRequest.UserName.Trim()));
            }
            if (!string.IsNullOrWhiteSpace(queryRequest.NickName))
            {
                filter = filter.And(x => x.NickName.Contains(queryRequest.NickName.Trim()));
            }
            if (!string.IsNullOrWhiteSpace(queryRequest.Mobile))
            {
                filter = filter.And(x => x.Mobile.Contains(queryRequest.Mobile));
            }
            if (queryRequest.Disabled.HasValue)
            {
                filter = filter.And(x => x.Disabled == queryRequest.Disabled);
            }
            IOrderedQueryable<SysUser> orderby(IQueryable<SysUser> x) => x.OrderByDescending(z => z.CreateTime);
            var sysUsers = base.Select(out int totalCount, filter, queryRequest.PageIndex, queryRequest.PageSize, orderby);
            if (sysUsers == null || sysUsers.Count() == 0)
                return new PageResult<SysUserResponse>()
                {
                    Rows = new List<SysUserResponse>(),
                    Total = 0
                };
            var sysRoles = _sysRoleService.GetRoles();
            var sysUserResponses = _mapper.Map<IEnumerable<SysUser>, IEnumerable<SysUserResponse>>(sysUsers);
            foreach (var sysUserResponse in sysUserResponses)
            {
                if (sysRoles != null)
                {
                    var sysUserRoles = _sysUserRoleService.GetUserRoles(sysUserResponse.Id);
                    if (sysUserRoles != null && sysUserRoles.Count() > 0)
                    {
                        var sysRoleIds = sysUserRoles.Select(x => x.RoleId);
                        var userRoles = sysRoles.Where(x => sysRoleIds.Contains(x.Id));
                        sysUserResponse.UserRoles = userRoles;
                    }
                }
                sysUserResponse.StatusDesc = GlobalEnum.GetEnumDescription((GlobalEnum.SysUserStatus)sysUserResponse.Status);
            }
            return new PageResult<SysUserResponse>()
            {
                Total = totalCount,
                Rows = sysUserResponses
            };
        }

        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public bool Delete(int userId, int optUserId)
        {
            var sysUser = base.Get(x => x.Id == userId);
            if (sysUser == null)
                throw new BusinessException("用户不存在");

            if (sysUser.IsDelete == (int)GlobalEnum.YESOrNO.Y)
                throw new BusinessException("用户已是删除状态");

            sysUser.IsDelete = (int)GlobalEnum.YESOrNO.Y;
            sysUser.UpdateTime = DateTime.Now;

            var ret = base.Update(sysUser);
            if (ret)
            {
                var optSysUser = this.GetUserInfoByUserId(optUserId);
                var operationLog = new OperationLog()
                {
                    UserId = optSysUser.UserId.ToString(),
                    UserName = optSysUser.NickName,
                    UserType = optSysUser.UserType,
                    BusinessParam = userId.ToString(),
                    LogContent = $"删除用户{ sysUser.NickName }",
                    ChangedFields = JsonConvert.SerializeObject(new
                    {
                        sysUser.UserId,
                        sysUser.UserName,
                        sysUser.NickName
                    }),
                    ResourceType = (int)GlobalEnum.ResourceType.DELETE_SYSUSER
                };
                _operationLogService.AddLog(operationLog);
            }
            return ret;
        }

        /// <summary>
        /// 禁用/启用
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public bool Disabled(int disabled, int userId, int optUserId)
        {
            var sysUser = base.Get(x => x.UserId == userId);
            if (sysUser == null)
                throw new BusinessException("用户不存在");
            if (sysUser.Disabled == disabled)
                throw new BusinessException("已成功保存为该状态，请勿重复操作");

            sysUser.Disabled = disabled;
            sysUser.UpdateTime = DateTime.Now;
            var ret = base.Update(sysUser);
            if (ret)
            {
                var optSysUser = this.GetUserInfoByUserId(optUserId);
                var operationLog = new OperationLog()
                {
                    UserId = optSysUser.UserId.ToString(),
                    UserName = optSysUser.NickName,
                    UserType = optSysUser.UserType,
                    BusinessParam = userId.ToString(),
                    LogContent = (disabled == (int)GlobalEnum.YESOrNO.Y ? "禁用" : "启用"),
                    ChangedFields = JsonConvert.SerializeObject(new
                    {
                        sysUser.UserId,
                        sysUser.UserName,
                        sysUser.NickName
                    }),
                    ResourceType = disabled == (int)GlobalEnum.YESOrNO.Y ? (int)GlobalEnum.ResourceType.SYSUSER_DISABLED : (int)GlobalEnum.ResourceType.SYSUSER_ENABLED
                };

                switch (sysUser.UserType)
                {
                    case (int)GlobalEnum.UserType.BACKSTAGE:
                        operationLog.LogContent += $"后台用户{sysUser.NickName}";
                        break;
                    case (int)GlobalEnum.UserType.CUSTOMER:
                        operationLog.LogContent += $"客户{sysUser.NickName}";
                        break;
                    case (int)GlobalEnum.UserType.USER:
                        operationLog.LogContent += $"注册用户{sysUser.NickName}";
                        break;
                    case (int)GlobalEnum.UserType.API:
                        operationLog.LogContent += $"API用户{sysUser.NickName}";
                        break;
                    default:
                        break;
                }
                _operationLogService.AddLog(operationLog);
            }
            return ret;
        }

        /// <summary>
        /// 保存用户信息
        /// </summary>
        /// <param name="addRequest"></param>
        /// <returns></returns>
        public bool SaveUser(SysUserAddRequest addRequest, int optUserId, out string addedUserId)
        {
            if (addRequest.Id <= 0)
            {
                var userExist = base.Get(x => x.UserName == addRequest.UserName && x.IsDelete == (int)GlobalEnum.YESOrNO.N);
                if (userExist != null)
                {
                    throw new BusinessException("用户名已存在");
                }
                if (addRequest.IsSubUser == (int)GlobalEnum.YESOrNO.Y && addRequest.ParentUserId != null && addRequest.ParentUserId > 0)
                {
                    var parentUser = base.Get(x => x.UserId == addRequest.ParentUserId);
                    if (parentUser.ParentUserId != null && parentUser.ParentUserId > 0)
                    {
                        throw new BusinessException("子账号无新增子账号权限");
                    }
                    if (parentUser.UserType != (int)GlobalEnum.UserType.CUSTOMER)
                    {
                        throw new BusinessException("非客户类型不能新增子账号");
                    }
                }
                int userNo = (int)base.GetSequence(_sequenceName);
                if (userNo <= 0)
                {
                    throw new BusinessException("用户编号生成失败");
                }
                var sysUser = _mapper.Map<SysUserAddRequest, SysUser>(addRequest);
                LogInfoWriter.GetInstance().Info($"SysUser/Add, sysUser.Pwd:{sysUser.Pwd}");
                sysUser.UserId = userNo.PadTo();
                sysUser.CreateTime = DateTime.Now;
                sysUser.Disabled = (int)GlobalEnum.YESOrNO.N;
                sysUser.Salt = Guid.NewGuid().ToString("N");
                sysUser.IsDelete = (int)GlobalEnum.YESOrNO.N;
                sysUser.Status = (int)GlobalEnum.SysUserStatus.NORMAL;
                sysUser.Pwd = Md5Util.Md5Encrypt(sysUser.Pwd + sysUser.Salt);
                LogInfoWriter.GetInstance().Info($"SysUser/Add,请求参数:{addRequest.ToJson()}, salt:{sysUser.Salt},加密之后的密码:{sysUser.Pwd}");
                var optSysUser = this.GetUserInfoByUserId(optUserId);
                if (addRequest.IsSubUser == (int)GlobalEnum.YESOrNO.Y && optSysUser != null)
                {
                    //子账号的用户类型和主账号保持一致
                    sysUser.UserType = optSysUser.UserType;
                    sysUser.UserPlatformId = optSysUser.UserPlatformId;
                }
                var ret = base.AddEntity(sysUser);
                if (ret)
                {
                    var msg = "新增用户";
                    if (addRequest.IsSubUser == (int)GlobalEnum.YESOrNO.Y)
                    {
                        //子用户权限
                        var subUserRole = _sysRoleService.GetRole(GlobalConst.SUBUSER_USER_ROLE);
                        if (subUserRole != null)
                            _sysUserRoleService.SaveUserRoles(sysUser.UserId, new List<int> { subUserRole.Id });
                        //子用户用户组
                        var subUserGroup = _sysUserGroupService.GetUserGroup(GlobalConst.SUBUSER_USER_ROLE);
                        if (subUserGroup != null)
                            _sysUserGroupUserService.SaveUserGroups(sysUser.UserId, new List<int> { subUserGroup.Id });
                        msg = "新增子用户";
                    }
                    if (addRequest.UserType == (int)GlobalEnum.UserType.USER)
                    {
                        //如果是注册用户 配置角色
                        var registerUserRole = _sysRoleService.GetRole(GlobalConst.REGISTER_USER_ROLE);
                        if (registerUserRole != null)
                        {
                            _sysUserRoleService.SaveUserRoles(sysUser.UserId, new List<int> { registerUserRole.Id });
                        }
                    }
                    //操作日志
                    if (optSysUser != null)
                    {
                        _operationLogService.AddLog(new OperationLog()
                        {
                            UserId = optSysUser.UserId.ToString(),
                            UserName = optSysUser.NickName,
                            UserType = optSysUser.UserType,
                            BusinessParam = sysUser.UserId.ToString(),
                            LogContent = $"{msg}:{ sysUser.NickName }",
                            ChangedFields = JsonConvert.SerializeObject(new { sysUser.UserId, sysUser.UserType, sysUser.UserName, sysUser.NickName }),
                            ResourceType = (int)GlobalEnum.ResourceType.NEW_SYSSUBUSER,
                        });
                    }
                    addedUserId = sysUser.UserId.ToString();
                }
                else
                {
                    addedUserId = string.Empty;
                }
                return ret;
            }
            else
            {
                var currentSysUser = base.Get(x => x.Id == addRequest.Id && x.IsDelete == (int)GlobalEnum.YESOrNO.N);
                var tempCurrentUser = currentSysUser.DeepClone();
                if (currentSysUser == null)
                {
                    throw new BusinessException("用户不存在");
                }
                if (!string.IsNullOrWhiteSpace(addRequest.Mail) && addRequest.CaptchaType != null && addRequest.CaptchaType == (int)GlobalEnum.CaptchaType.CHANGEUSERINFO)
                {
                    try
                    {
                        if (string.IsNullOrWhiteSpace(addRequest.Code))
                        {
                            throw new BusinessException("请输入验证码");
                        }
                        var res = _captchaService.VerifyMailCaptcha(addRequest.Mail, addRequest.Code, (GlobalEnum.CaptchaType)addRequest.CaptchaType);

                    }
                    catch (Exception ex)
                    {
                        throw new BusinessException(ex.Message);
                    }
                }
                if (!string.IsNullOrWhiteSpace(addRequest.Avatar))
                {
                    currentSysUser.Avatar = addRequest.Avatar;
                }
                currentSysUser.Mail = addRequest.Mail;
                currentSysUser.NickName = addRequest.NickName;
                currentSysUser.Mobile = addRequest.Mobile;
                currentSysUser.UpdateTime = DateTime.Now;
                //修改过的属性映射
                if (!string.IsNullOrWhiteSpace(addRequest.Pwd))
                {
                    var pwd = Md5Util.Md5Encrypt(addRequest.Pwd + currentSysUser.Salt);
                    if (pwd != currentSysUser.Pwd)
                    {
                        var salt = Guid.NewGuid().ToString("N");
                        currentSysUser.Pwd = Md5Util.Md5Encrypt(addRequest.Pwd + salt);
                        currentSysUser.Salt = salt;
                    }
                }
                addedUserId = currentSysUser.UserId.ToString();
                var ret = base.Update(currentSysUser);
                if (ret)
                {
                    var msg = "修改用户";
                    if (addRequest.IsSubUser == (int)GlobalEnum.YESOrNO.Y)
                    {
                        msg = "修改子用户";
                    }
                    var optSysUser = this.GetUserInfoByUserId(optUserId);
                    _operationLogService.AddLog(new OperationLog()
                    {
                        UserId = optSysUser.UserId.ToString(),
                        UserName = optSysUser.NickName,
                        UserType = optSysUser.UserType,
                        BusinessParam = currentSysUser.UserId.ToString(),
                        LogContent = $"{msg}{ currentSysUser.NickName }",
                        ChangedFields = tempCurrentUser.GetChangedFields(currentSysUser),
                        ResourceType = (int)GlobalEnum.ResourceType.EDIT_SYSUSER,
                    });
                }
                return ret;
            }
        }

        /// <summary>
        /// 获取用户SESSION缓存
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public SysUserSession GetUserSessionCache(string token)
        {
            return this._sysUserCacheProvider.GetSession(token);
        }

        /// <summary>
        /// 获取TOKEN缓存
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public string GetUserTokenCache(string userId)
        {
            return this._sysUserCacheProvider.GetToken(userId);
        }

        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <returns></returns>
        public SysUserResponse GetUserInfo(int userId, bool includePower = false)
        {
            var sysUser = base.Get(x => x.UserId == userId && x.IsDelete == (int)GlobalEnum.YESOrNO.N);
            if (sysUser == null)
                throw new BusinessException("用户信息不存在");
            if (sysUser.UserType == (int)GlobalEnum.UserType.USER)
            {
                var userRoles = _sysUserRoleService.GetUserRoles(sysUser.Id);
                if (userRoles == null || userRoles.Count() == 0)
                {
                    Func<SysUserResponse, SysUserResponse> registerCfg = (userResponse) =>
                     {
                         //访客角色
                         var registerUserRoles = new List<SysRoleResponse>()
                         {
                            new SysRoleResponse()
                            {
                                Id = -100,
                                RoleName = "注册用户",
                                Meta = "register",
                                Description ="注册用户",
                            }
                         };

                         //注册用户菜单
                         var registerMenus = new List<SysMenuResponse>() {
                            new SysMenuResponse()
                            {
                               Id = -100,
                               Title = "控制台",
                               Name = "Root",
                               Path = "/",
                               Component = "Layout",
                               Sort= 1,
                               Description ="访客",
                               Icon="dashboard",
                               Redirect = "/dashboard",
                               Visiable = (int)GlobalEnum.YESOrNO.Y,
                               AlwaysShow = (int)GlobalEnum.YESOrNO.N,
                               Enabled = (int)GlobalEnum.YESOrNO.Y,
                               Level = (int)GlobalEnum.MenuLevel.LEVEL1
                            },
                            new SysMenuResponse()
                            {
                               Id = -101,
                               ParentId = -100,
                               Title = "控制台",
                               Name = "Dashboard",
                               Path = "/dashboard",
                               Component = "Dashboard",
                               Sort = 1,
                               Description ="访客",
                               Icon ="dashboard",
                               BreadcrumbShow = (int)GlobalEnum.YESOrNO.Y,
                               Visiable = (int)GlobalEnum.YESOrNO.Y,
                               Enabled = (int)GlobalEnum.YESOrNO.Y,
                               AlwaysShow = (int)GlobalEnum.YESOrNO.Y,
                               Level = (int)GlobalEnum.MenuLevel.LEVEL2
                            },
                            new SysMenuResponse()
                            {
                               Id = -110,
                               Title = "采购管理",
                               Name = "Purchase",
                               Path = "/purchase",
                               Component = "Purchase",
                               Sort = 1,
                               Description = "访客",
                               Icon="purchase",
                               Redirect = "/hotellist",
                               BreadcrumbShow = (int)GlobalEnum.YESOrNO.Y,
                               AlwaysShow = (int)GlobalEnum.YESOrNO.N,
                               Visiable = (int)GlobalEnum.YESOrNO.Y,
                               Enabled = (int)GlobalEnum.YESOrNO.Y,
                               Level = (int)GlobalEnum.MenuLevel.LEVEL1
                            },
                            new SysMenuResponse()
                            {
                               Id = -111,
                               ParentId = -110,
                               Description ="访客",
                               Title = "酒店列表",
                               Name = "HotelList",
                               Path = "/hotellist",
                               Component = "HotelList",
                               Sort = 1,
                               Icon ="hotel",
                               Visiable = (int)GlobalEnum.YESOrNO.Y,
                               BreadcrumbShow = (int)GlobalEnum.YESOrNO.Y,
                               Enabled = (int)GlobalEnum.YESOrNO.Y,
                               AlwaysShow = (int)GlobalEnum.YESOrNO.Y,
                               Level = (int)GlobalEnum.MenuLevel.LEVEL2
                            }
                         };

                         //访客模块
                         var registerModules = new List<SysModuleResponse>();
                         userResponse.UserRoles = registerUserRoles;
                         userResponse.RoleMenus = registerMenus;
                         userResponse.Modules = registerModules;
                         return userResponse;
                     };

                    var sysUserResponse = this._mapper.Map<SysUser, SysUserResponse>(sysUser);
                    sysUserResponse.StatusDesc = GlobalEnum.GetEnumDescription((GlobalEnum.SysUserStatus)sysUserResponse.Status);
                    sysUserResponse.UserTypeDesc = GlobalEnum.GetEnumDescription((GlobalEnum.UserType)sysUserResponse.UserType);
                    return registerCfg(sysUserResponse);
                }

                // 系统用户如果没有客户视为访客权限
                //if (string.IsNullOrWhiteSpace(sysUser.CustomerNo))
                //{
                //    return visiterCfg(sysUserResponse);
                //}
                //else
                //{
                //    var customer = _customerService.GetCustomer(sysUser.CustomerNo);
                //    if (customer.Status == (int)GlobalEnum.CustomerStatus.ORIGINAL || customer.Status == (int)GlobalEnum.CustomerStatus.IMPOWER)
                //    {
                //        return visiterCfg(sysUserResponse);
                //    }
                //}
            }

            {
                //用户
                var userResponse = this._mapper.Map<SysUser, SysUserResponse>(sysUser);
                userResponse.StatusDesc = GlobalEnum.GetEnumDescription((GlobalEnum.SysUserStatus)userResponse.Status);
                userResponse.UserTypeDesc = GlobalEnum.GetEnumDescription((GlobalEnum.UserType)userResponse.UserType);

                if (sysUser.UserType == (int)GlobalEnum.UserType.BACKSTAGE)
                {
                    var defaultPwd = _sysConfigService.GetSysUserDefaultPwd();
                    var mdPwd = Md5Util.Md5Encrypt($"{defaultPwd}{sysUser.UserName}");
                    string pwd = Md5Util.Md5Encrypt($"{mdPwd}{sysUser.Salt}");
                    userResponse.IsDefaultPwd = pwd == sysUser.Pwd;
                }

                if (includePower)
                {
                    //用户角色
                    var userRoles = _sysUserRoleService.GetUserRoles(sysUser.Id);

                    //角色权限
                    var roleIds = userRoles.Select(x => x.RoleId);
                    var roles = this._sysRoleService.GetRoles(roleIds);
                    var allRolePermissions = _sysRolePermissionService.GetRolePermissions(roleIds);

                    //权限菜单
                    var menuIds = allRolePermissions.Select(x => x.PermissionId).Distinct();
                    var menus = _sysMenuService.GetMenus(menuIds);

                    //用户组
                    var userGroups = _sysUserGroupUserService.GetUserGroups(sysUser.Id);
                    var userGroupIds = userGroups.Select(x => x.UserGroupId);
                    //模块
                    var userGroupModules = _sysUserGroupModuleService.GetUserGroupModules(userGroupIds);
                    var moduleIds = userGroupModules.Select(x => x.ModuleId);
                    var modules = _sysModuleService.GetSysModules(moduleIds);

                    userResponse.UserRoles = roles;
                    userResponse.RoleMenus = menus;
                    userResponse.Modules = modules;
                }

                return userResponse;
            }
        }

        /// <summary>
        /// 根据UserId获取用户信息
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public SysUser GetUserInfoByUserId(int userId)
        {
            if (_systemUsers.TryGetValue(userId, out SysUser sysUser))
                return sysUser;

            var sysUserInfo = base.Get(x => x.UserId == userId);
            if (sysUserInfo == null)
                throw new BusinessException($"{userId}用户信息不存在");

            return sysUserInfo;
        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="pwd"></param>
        /// <returns></returns>
        public string Login(string userName, string pwd, string seed, string userAgent, string clientIP)
        {
            Expression<Func<SysUser, bool>> filter = x => x.UserName == userName.Trim() || x.Mobile == userName;
            //filter = filter.And(x => x.Disabled == (int)GlobalEnum.YESOrNO.N);
            var sysUser = base.Get(filter);
            if (sysUser == null)
                throw new BusinessException("用户名或密码不正确");
            if (sysUser.Disabled == (int)GlobalEnum.YESOrNO.Y)
                throw new BusinessException("您的账号已失效，请联系客服");
            int loginErrorCount = _sysConfigService.GetLoginErrorLimitCount();
            if (sysUser.LoginErrorCount >= loginErrorCount)
            {
                if (sysUser.LoginErrorTime.HasValue)
                {
                    if (sysUser.LoginErrorTime.Value.AddMinutes(1) > DateTime.Now)
                    {
                        throw new BusinessException("用户名或密码错误次数过多,请稍后重试");
                    }
                }
            }
            var pwdEncrypt = Md5Util.Md5Encrypt(pwd + sysUser.Salt);
            if (sysUser.Pwd != pwdEncrypt)
            {
                Task.Run(() =>
                {
                    AfterLogin(sysUser, userAgent, clientIP, true);
                });
                throw new BusinessException("用户名或密码不正确");
            }

            //Task.Run(() =>
            //{
            AfterLogin(sysUser, userAgent, clientIP);
            //});

            //用户私钥
            var privateKey = this.GeneratePrivateKey(sysUser);

            //用户唯一标识
            var sysUserResponse = _mapper.Map<SysUser, SysUserClient>(sysUser);
            var sysUserToken = AESUtil.AesEncrypt(JsonConvert.SerializeObject(sysUserResponse), Md5Util.Md5Encrypt(privateKey), seed);

            //用户信息
            var sysUserSession = _mapper.Map<SysUser, SysUserSession>(sysUser);
            sysUserSession.PrivateKey = privateKey;
            sysUserSession.Seed = seed;
            sysUserSession.Date = DateTime.Now;
            sysUserSession.ClientIP = clientIP;
            sysUserSession.Token = sysUserToken;
            {
                if (sysUser.UserType != (int)GlobalEnum.UserType.BACKSTAGE)
                {
                    var lastSysUserToken = this._sysUserCacheProvider.GetToken(sysUser.UserId.ToString());
                    if (!string.IsNullOrWhiteSpace(lastSysUserToken))
                    {
                        this._sysUserCacheProvider.RemoveSession(lastSysUserToken);
                    }
                }
            }
            {
                this._sysUserCacheProvider.SetSession(sysUserToken, sysUserSession);
                this._sysUserCacheProvider.SetToken(sysUser.UserId.ToString(), sysUserToken);
            }

            return $"{sysUserToken}.{privateKey}";
        }

        /// <summary>
        /// 登出
        /// </summary>
        /// <param name="sysUserSession"></param>
        /// <param name="userAgent"></param>
        /// <param name="clientIP"></param>
        /// <returns></returns>
        public bool Logout(SysUserSession sysUserSession, string userAgent, string clientIP)
        {
            bool ret = _sysUserCacheProvider.RemoveSession(sysUserSession.Token);
            if (ret)
            {
                this._sysUserCacheProvider.RemoveToken(sysUserSession.UserId.ToString());

                Task.Run(() =>
                {
                    _loginLogService.Add(new LoginLog()
                    {
                        UserId = sysUserSession.UserId,
                        UserName = sysUserSession.UserName,
                        LoginIP = clientIP,
                        LoginDevice = userAgent,
                        LoginRemark = sysUserSession.Token,
                        LoginType = (int)GlobalEnum.LoginType.Logout
                    });
                });
            }
            return ret;
        }

        /// <summary>
        /// 生成私钥
        /// </summary>
        /// <param name="sysUser"></param>
        /// <returns></returns>
        private string GeneratePrivateKey(SysUser sysUser)
        {
            string timeSeed = sysUser.CreateTime.ToString("yyyyMMddhhmmss");
            if (sysUser.LastLoginTime.HasValue)
                timeSeed = sysUser.LastLoginTime.Value.ToString("yyyyMMddhhmmss");

            var encryptKey = $"{sysUser.UserId}|{timeSeed}|{sysUser.UserId}";
            var encryptVal = $"{sysUser.UserId.ToString()}|{sysUser.Pwd}";
            return AESUtil.AesEncrypt(encryptVal, encryptKey);
        }

        /// <summary>
        /// 登录操作
        /// </summary>
        /// <param name="sysUser"></param>
        public bool AfterLogin(SysUser sysUser, string userAgent, string ip, bool hasError = false)
        {
            if (hasError)
            {
                sysUser.LoginErrorTime = DateTime.Now;
                sysUser.LoginErrorCount = sysUser.LoginErrorCount.HasValue ? sysUser.LoginErrorCount += 1 : 1;
            }
            else
            {
                sysUser.LoginErrorTime = null;
                sysUser.LoginErrorCount = 0;
            }
            sysUser.LoginCount = sysUser.LoginCount.HasValue ? sysUser.LoginCount += 1 : 1;
            sysUser.LastLoginTime = DateTime.Now;
            var ret = base.Update(sysUser);
            if (ret)
            {
                //_loginLogService.Add(new LoginLog()
                //{
                //    UserId = sysUser.UserId,
                //    UserName = sysUser.UserName,
                //    LoginIP = ip,
                //    LoginDevice = userAgent,
                //    LoginType = (int)GlobalEnum.LoginType.Login
                //});
            }
            return ret;
        }

        /// <summary>
        /// 检查用户名是否存在
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public bool CheckUserNameIsExist(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
                throw new BusinessException("用户名不能为空");

            return base.Get(x => x.UserName == userName) != null;
        }

        /// <summary>
        /// 检查手机号是否存在
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public bool CheckUserMobileIsExist(string mobile)
        {
            if (string.IsNullOrWhiteSpace(mobile))
                throw new BusinessException("手机号不能为空");

            var sysUser = base.Get(x => x.Mobile == mobile);

            //if (sysUser != null)
            //{
            //    if (sysUser.UserType != (int)GlobalEnum.UserType.CUSTOMER)
            //    {
            //        throw new BusinessException("用户类型无效");
            //    }
            //}

            return sysUser != null;
        }

        public bool CheckSysUserMobileIsExist(int userId, string mobile)
        {
            if (string.IsNullOrWhiteSpace(mobile))
                throw new BusinessException("手机号不能为空");

            var sysUser = base.Get(x => x.IsDelete == (int)GlobalEnum.YESOrNO.N &&
                                x.Mobile == mobile && x.UserId != userId);
            return sysUser != null;
        }

        /// <summary>
        /// 设置客户编号
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="customerId"></param>
        /// <returns></returns>
        public bool SetCustomer(int userId, string customerNo, string nickName)
        {
            var sysUser = base.Get(x => x.UserId == userId);
            if (sysUser != null && sysUser.UserType == (int)GlobalEnum.UserType.USER)
            {
                //如果是从注册用户完善信息变为客户，删除注册角色时分配的权限 重新分配客户权限
                var userRoleIds = _sysUserRoleService.GetUserRoles(sysUser.Id);
                var ids = userRoleIds?.Select(x => x.Id).ToList();
                _sysUserRoleService.DeleteUserRoles(ids);
            }
            //用户权限
            var customerRole = _sysRoleService.GetRole(GlobalConst.CUSTOMER_USER_ROLE);
            if (customerRole != null)
                _sysUserRoleService.SaveUserRoles(sysUser.UserId, new List<int> { customerRole.Id });
            var customerUserGroup = _sysUserGroupService.GetUserGroup(GlobalConst.CUSTOMER_USER_USERGROUP);
            if (customerUserGroup != null)
                _sysUserGroupUserService.SaveUserGroups(sysUser.UserId, new List<int> { customerUserGroup.Id });
            sysUser.NickName = nickName;
            sysUser.CustomerNo = customerNo;
            sysUser.UserType = (int)GlobalEnum.UserType.CUSTOMER;
            sysUser.UpdateTime = DateTime.Now;
            return base.Update(sysUser);
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="pwd"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public bool ChangePwd(string pwd, string oldPwd, int userId)
        {
            var sysUser = base.Get(x => x.UserId == userId);
            if (sysUser == null)
                throw new BusinessException("未找到用户信息");

            if (sysUser.Pwd != Md5Util.Md5Encrypt(oldPwd + sysUser.Salt))
            {
                throw new BusinessException("原密码不正确");
            }

            sysUser.Salt = Guid.NewGuid().ToString("N");
            var pwdEncrypt = Md5Util.Md5Encrypt(pwd + sysUser.Salt);
            sysUser.Pwd = pwdEncrypt;
            sysUser.LoginErrorCount = 0;
            sysUser.UpdateTime = DateTime.Now;
            var ret = base.Update(sysUser);
            if (ret)
            {
                _operationLogService.AddLog(new OperationLog()
                {
                    UserId = sysUser.UserId.ToString(),
                    UserName = sysUser.NickName,
                    UserType = sysUser.UserType,
                    BusinessParam = sysUser.UserId.ToString(),
                    LogContent = $"修改密码{ sysUser.NickName }",
                    ChangedFields = JsonConvert.SerializeObject(new
                    {
                        sysUser.UserId,
                        sysUser.UserName,
                        sysUser.NickName
                    }),
                    ResourceType = (int)GlobalEnum.ResourceType.CHANGE_PWD,
                });
            }
            return ret;
        }

        public bool ChangePwd(string pwd, int userId)
        {
            var sysUser = base.Get(x => x.UserId == userId);

            var pwdEncrypt = Md5Util.Md5Encrypt(pwd + sysUser.Salt);

            sysUser.Pwd = pwdEncrypt;
            sysUser.LoginErrorCount = 0;
            sysUser.UpdateTime = DateTime.Now;
            var ret = base.Update(sysUser);
            return ret;
        }

        /// <summary>
        /// 重置密码
        /// </summary>
        /// <param name="mobile"></param>
        /// <param name="requestId"></param>
        /// <returns></returns>
        public bool ResetPwd(string mobile, string pwd, string requestId)
        {
            var sysUser = base.Get(x => x.Mobile == mobile);
            if (sysUser == null)
            {
                throw new BusinessException("用户不存在");
            }
            if (sysUser.Status == (int)GlobalEnum.SysUserStatus.LOCK)
            {
                throw new BusinessException("用户已被锁定");
            }
            if (sysUser.UserType != (int)GlobalEnum.UserType.CUSTOMER && sysUser.UserType != (int)GlobalEnum.UserType.USER)
            {
                throw new BusinessException("当前用户不能找回密码");
            }

            var captcha = _captchaService.GetCaptcha(sysUser.Mobile, requestId);
            if (captcha == null)
                throw new BusinessException("验证失败");

            var ret = this.ChangePwd(pwd, sysUser.UserId);
            if (ret)
            {
                _captchaService.IsUse(captcha);

                _operationLogService.AddLog(new OperationLog()
                {
                    UserId = sysUser.UserId.ToString(),
                    UserName = sysUser.NickName,
                    UserType = sysUser.UserType,
                    BusinessParam = sysUser.UserId.ToString(),
                    LogContent = $"找回密码{ sysUser.NickName }",
                    ChangedFields = JsonConvert.SerializeObject(new
                    {
                        sysUser.UserId,
                        sysUser.UserName,
                        sysUser.NickName
                    }),
                    ResourceType = (int)GlobalEnum.ResourceType.FIND_PWD,
                });
            }
            return ret;
        }

        /// <summary>
        /// 找回密码
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public bool FindPwd(string mobile, string clientIP, string randstr, string ticket, int ret)
        {
            var sysUser = base.Get(x => x.Mobile == mobile);
            if (sysUser == null)
            {
                throw new BusinessException("用户不存在");
            }
            if (sysUser.Status == (int)GlobalEnum.SysUserStatus.LOCK)
            {
                throw new BusinessException("用户已被锁定");
            }
            if (sysUser.UserType != (int)GlobalEnum.UserType.CUSTOMER && sysUser.UserType != (int)GlobalEnum.UserType.USER)
            {
                throw new BusinessException("当前用户不能找回密码");
            }
            //滑动验证先注释
            //var verRet = _captchaService.SlideVerification(mobile, randstr, ticket, ret, clientIP, GlobalEnum.CaptchaType.FINDPWD);
            //if (verRet.Result)
            //{
            //    return _captchaService.SendCaptcha(mobile, string.Empty, verRet.RequestId, GlobalEnum.CaptchaType.FINDPWD, clientIP);
            //}
            return _captchaService.SendCaptcha(mobile, string.Empty, "11324165465454121", GlobalEnum.CaptchaType.FINDPWD, clientIP);
            // return false;
        }

        /// <summary>
        /// 查询用户手机号
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public string GetUserMobile(string userName)
        {
            var sysUser = base.Get(x => x.UserName == userName);
            if (sysUser == null)
            {
                throw new BusinessException("用户不存在");
            }
            if (sysUser.Status == (int)GlobalEnum.SysUserStatus.LOCK)
            {
                throw new BusinessException("用户已被锁定");
            }
            return sysUser.Mobile;
        }

        /// <summary>
        /// 查询用户
        /// </summary>
        /// <param name="uname"></param>
        /// <returns></returns>
        public SysUser GetSysUser(string uname, GlobalEnum.UserType userType = GlobalEnum.UserType.ALL)
        {
            return base.Get(x => x.UserName == uname && x.UserType == (int)userType);
        }

        /// <summary>
        /// 根据手机号查询用户
        /// </summary>
        /// <param name="uname"></param>
        /// <returns></returns>
        public SysUser GetSysUserByMobile(string mobile)
        {
            return base.Get(x => x.Mobile == mobile);
        }

        /// <summary>
        /// 查询客户编号
        /// </summary>
        /// <param name="uname"></param>
        /// <returns></returns>
        public string GetCustomerNo(string mobile)
        {
            var customer = base.Get(x => x.Mobile == mobile && x.Disabled == (int)GlobalEnum.YESOrNO.N);
            if (customer == null)
            {
                throw new BusinessException("用户不存在");
            }
            if (customer.UserType != (int)GlobalEnum.UserType.CUSTOMER && customer.UserType != (int)GlobalEnum.UserType.USER)
            {
                throw new BusinessException("用户类型无效");
            }

            return customer?.CustomerNo;
        }

        /// <summary>
        /// 查询接口用户
        /// </summary>
        /// <returns></returns>
        public SysUser GetApiUser(string userName, string pwd)
        {
            var sysUser = base.Get(x => x.UserType == (int)GlobalEnum.UserType.API && x.UserName == userName);
            if (sysUser == null)
            {
                return null;
            }
            string md5 = Md5Util.Md5Encrypt(pwd + sysUser.Salt);
            if (sysUser.Pwd != md5)
            {
                return null;
            }
            return sysUser;
        }

        /// <summary>
        /// Api接口登录
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="pwd"></param>
        /// <param name="seed"></param>
        /// <param name="clientIP"></param>
        /// <returns></returns>
        public bool ApiLogin(string userName, string pwd, string clientIP)
        {
            Expression<Func<SysUser, bool>> filter = x => (x.UserType == (int)GlobalEnum.UserType.API || x.UserType == (int)GlobalEnum.UserType.CUSTOMER) && x.UserName == userName;
            var sysUser = base.Get(filter);
            if (sysUser == null)
                return false;

            var pwdEncrypt = Md5Util.Md5Encrypt(pwd + sysUser.Salt);
            if (sysUser.Pwd != pwdEncrypt)
            {
                Task.Run(() =>
                {
                    AfterLogin(sysUser, string.Empty, clientIP, true);
                });
                return false;
            }
            if (sysUser.LoginErrorCount >= 10)
            {
                return false;
            }

            Task.Run(() =>
            {
                AfterLogin(sysUser, string.Empty, clientIP);
            });

            return true;
        }

        /// <summary>
        /// 重置密码
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public bool ResetPwd(int userId, int optUserId)
        {
            var sysUser = base.Get(x => x.UserType == (int)GlobalEnum.UserType.BACKSTAGE && x.UserId == userId);
            if (sysUser == null)
            {
                throw new BusinessException("未找到用户");
            }

            var defaultPwd = _sysConfigService.GetSysUserDefaultPwd();

            string pwd = Md5Util.Md5Encrypt($"{defaultPwd}{sysUser.UserName}");

            sysUser.Salt = Guid.NewGuid().ToString("N");
            sysUser.Pwd = Md5Util.Md5Encrypt($"{pwd}{sysUser.Salt}");
            sysUser.UpdateTime = DateTime.Now;
            sysUser.LoginErrorCount = 0;
            sysUser.LoginErrorTime = null;
            var ret = base.Update(sysUser);
            if (ret)
            {
                var optSysUser = this.GetUserInfoByUserId(optUserId);
                _operationLogService.AddLog(new OperationLog()
                {
                    UserId = optSysUser.UserId.ToString(),
                    UserName = optSysUser.NickName,
                    UserType = optSysUser.UserType,
                    BusinessParam = userId.ToString(),
                    LogContent = $"重置密码{ sysUser.NickName }",
                    ChangedFields = JsonConvert.SerializeObject(new
                    {
                        sysUser.Id,
                        sysUser.UserName,
                        sysUser.NickName
                    }),
                    ResourceType = (int)GlobalEnum.ResourceType.SYSUSER_RESETPWD,
                });
            }
            return ret;
        }

        /// <summary>
        /// 微信登录
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="pwd"></param>
        /// <param name="seed"></param>
        /// <param name="userAgent"></param>
        /// <param name="clientIP"></param>
        /// <returns></returns>
        public string WechatLogin(string code, string seed, string userAgent, string clientIP)
        {
            int userId = _weChatService.GetWechatBindUser(code);
            Expression<Func<SysUser, bool>> filter = x => x.UserId == userId;
            var sysUser = base.Get(filter);
            if (sysUser == null)
                throw new BusinessException("用户未绑定微信登录");
            if (sysUser.Disabled == (int)GlobalEnum.YESOrNO.Y)
                throw new BusinessException("您的账号已失效，请联系客服");

            int loginErrorCount = _sysConfigService.GetLoginErrorLimitCount();
            if (sysUser.LoginErrorCount >= loginErrorCount)
            {
                if (sysUser.LoginErrorTime.HasValue)
                {
                    if (sysUser.LoginErrorTime.Value.AddMinutes(1) > DateTime.Now)
                    {
                        throw new BusinessException("用户名或密码错误次数过多,请稍后重试");
                    }
                }
            }

            Task.Run(() =>
            {
                AfterLogin(sysUser, userAgent, clientIP);
            });

            //用户私钥
            var privateKey = this.GeneratePrivateKey(sysUser);

            //用户唯一标识
            var sysUserResponse = _mapper.Map<SysUser, SysUserClient>(sysUser);
            var sysUserToken = AESUtil.AesEncrypt(JsonConvert.SerializeObject(sysUserResponse), Md5Util.Md5Encrypt(privateKey), seed);

            //用户信息
            var sysUserSession = _mapper.Map<SysUser, SysUserSession>(sysUser);
            sysUserSession.PrivateKey = privateKey;
            sysUserSession.Seed = seed;
            sysUserSession.Date = DateTime.Now;
            sysUserSession.ClientIP = clientIP;
            sysUserSession.Token = sysUserToken;
            {
                if (sysUser.UserType != (int)GlobalEnum.UserType.BACKSTAGE)
                {
                    var lastSysUserToken = this._sysUserCacheProvider.GetToken(sysUser.UserId.ToString());
                    if (!string.IsNullOrWhiteSpace(lastSysUserToken))
                    {
                        this._sysUserCacheProvider.RemoveSession(lastSysUserToken);
                    }
                }
            }
            {
                this._sysUserCacheProvider.SetSession(sysUserToken, sysUserSession);
                this._sysUserCacheProvider.SetToken(sysUser.UserId.ToString(), sysUserToken);
            }

            return $"{sysUserToken}.{privateKey}";
        }


        /// <summary>
        /// 根据主账号Id获取子用户Id列表
        /// </summary>
        /// <param name="deptId"></param>
        /// <returns></returns>
        public IEnumerable<int> GetUserIdsByParentId(int parentUserId)
        {
            return base.Select(x => x.IsDelete == (int)GlobalEnum.YESOrNO.N && x.ParentUserId == parentUserId).Select(x => x.UserId);

        }

        /// <summary>
        /// 根据部门获取用户
        /// </summary>
        /// <param name="deptId"></param>
        /// <returns></returns>
        public IEnumerable<SysUserResponse> GetUsersByDeptId(int[] deptIds)
        {
            var deptIdStrs = string.Join(",", deptIds);
            deptIdStrs = $"{ deptIdStrs },";

            var sysUsers = base.Select(x => x.IsDelete == (int)GlobalEnum.YESOrNO.N && x.UserType == (int)GlobalEnum.UserType.BACKSTAGE && x.DepartmentId.Contains(deptIdStrs));
            return _mapper.Map<IEnumerable<SysUser>, IEnumerable<SysUserResponse>>(sysUsers);
        }

        /// <summary>
        /// 同步部门员工
        /// </summary>
        /// <param name="deptIds"></param>
        public void SyncEmployees(int[] deptIds = null)
        {
            var departments = _sysDepartmentService.GetDepartments(deptIds);

            if (departments != null && departments.Count() > 0)
            {
                foreach (var dept in departments)
                {
                    if (!dept.ThirdId.HasValue)
                    {
                        continue;
                    }

                    try
                    {
                        var weixinSetting = Senparc.Weixin.Config.SenparcWeixinSetting.WorkSetting;
                        var qyDepartmentsRes = MailListApi.GetDepartmentMember(AccessTokenContainer.TryGetToken(weixinSetting.WeixinCorpId, weixinSetting.WeixinCorpSecret), dept.ThirdId.Value, 0);

                        if (qyDepartmentsRes.errcode != Senparc.Weixin.ReturnCode_Work.请求成功)
                        {
                            continue;
                        }

                        if (qyDepartmentsRes.userlist == null || qyDepartmentsRes.userlist.Count == 0)
                        {
                            continue;
                        }

                        var depaerments = _sysDepartmentService.GetDepartments();
                        foreach (var user in qyDepartmentsRes.userlist)
                        {
                            var sysUsers = base.Select(x => x.IsDelete == (int)GlobalEnum.YESOrNO.N && x.UserType == (int)GlobalEnum.UserType.BACKSTAGE && x.NickName == user.name);
                            if (sysUsers == null || sysUsers.Count() == 0 || sysUsers.Count() > 1)
                            {
                                continue;
                            }

                            var sysUser = sysUsers.FirstOrDefault();
                            sysUser.QyWorkUserId = user.userid;

                            var userDepartments = depaerments.Where(x => user.department.Contains(x.ThirdId.Value));
                            if (userDepartments != null && userDepartments.Count() > 0)
                            {
                                var userDeptIds = userDepartments.Select(x => x.Id);
                                var userDeptIdStr = string.Join(",", userDeptIds);
                                userDeptIdStr = $"{ userDeptIdStr },";

                                sysUser.DepartmentId = userDeptIdStr;
                            }
                            base.Update(sysUser);
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }
        }
    }
}
