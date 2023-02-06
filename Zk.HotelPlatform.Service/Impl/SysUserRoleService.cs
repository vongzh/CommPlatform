using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zk.HotelPlatform.DBProvider;
using Zk.HotelPlatform.Model;
using Zk.HotelPlatform.Utils;
using Zk.HotelPlatform.Utils.Global;

namespace Zk.HotelPlatform.Service.Impl
{
    public class SysUserRoleService : DataService<SysUserRole>, ISysUserRoleService
    {
        private readonly IOperationLogService _operationLogService = null;
        private readonly ISysRoleService _sysRoleService = null;

        public ISysUserService SysUserService { get; set; }

        public SysUserRoleService(IDataProvider<SysUserRole> dataProvider, ISysRoleService sysRoleService, IOperationLogService operationLogService)
            : base(dataProvider)
        {
            _operationLogService = operationLogService;
            _sysRoleService = sysRoleService;
        }

        /// <summary>
        /// 查询用户角色
        /// </summary>
        /// <param name="userId">用户表的ID（非UserId）</param>
        /// <returns></returns>
        public IEnumerable<SysUserRole> GetUserRoles(int userId)
        {
            return base.Select(x => x.UserId == userId);
        }

        public IEnumerable<SysUserRole> DeleteUserRoles(IEnumerable<int> Ids)
        {
           return  base.DeleteAll(x => Ids.Contains(x.Id));
           
        }

        /// <summary>
        /// 设置用户角色
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="roles"></param>
        /// <returns></returns>
        public bool SaveUserRoles(int userId, IEnumerable<int> roles, int? optUserId = null)
        {
            var sysUser = SysUserService.GetUserInfoByUserId(userId);
            if (sysUser == null)
                throw new BusinessException("未找到用户信息");
            var sysRoles = _sysRoleService.GetRoles();
            if (sysRoles == null || sysRoles.Count() <= 0)
                throw new BusinessException("未找到可分配的角色");
            // 过滤已删除的角色
            var sysRoleIds = sysRoles.Select(x => x.Id);
            roles = roles.Where(x => sysRoleIds.Contains(x));
            var currentUserRoles = base.Select(x => x.UserId == sysUser.Id).ToList();
            if (currentUserRoles == null || currentUserRoles.Count <= 0)
            {
                roles.ToList().ForEach(role =>
                {
                    base.AddEntity(new SysUserRole()
                    {
                        RoleId = role,
                        UserId = sysUser.Id
                    });
                });

                if (optUserId.HasValue)
                {
                    var addSysRoles = _sysRoleService.GetRoles(roles);
                    string roleNames = string.Join(",", addSysRoles.Select(x => new { x.Id, x.RoleName }));

                    var optSysUser = SysUserService.GetUserInfoByUserId(optUserId.Value);
                    _operationLogService.AddLog(new OperationLog()
                    {
                        UserId = optSysUser.UserId.ToString(),
                        UserName = optSysUser.NickName,
                        UserType = optSysUser.UserType,
                        BusinessParam = sysUser.UserId.ToString(),
                        LogContent = $"用户{ sysUser.NickName }的角色修改",
                        ChangedFields = $"新增角色[{roleNames}]",
                        ResourceType = (int)GlobalEnum.ResourceType.SYSUSER_ROLE_EDIT,
                    });
                }
                return true;
            }
            else
            {
                var userRoleIds = currentUserRoles.Select(x => x.RoleId);
                var addRoles = roles.Except(userRoleIds).ToList();
                if (addRoles != null && addRoles.Any())
                {
                    addRoles.ForEach(role =>
                    {
                        base.AddEntity(new SysUserRole()
                        {
                            RoleId = role,
                            UserId = sysUser.Id
                        });
                    });
                }
                var deleteRoles = userRoleIds.Except(roles).ToList();
                if (deleteRoles != null && deleteRoles.Any())
                {
                    deleteRoles.ForEach(x =>
                    {
                        base.DeleteAll(z => z.UserId == sysUser.Id && deleteRoles.Contains(z.RoleId));
                    });
                }
                if (optUserId.HasValue)
                {
                    StringBuilder changed = new StringBuilder();

                    if (addRoles != null && addRoles.Count > 0)
                    {
                        var addSysRoles = _sysRoleService.GetRoles(addRoles);
                        string addRoleNames = string.Join(",", addSysRoles.Select(x => new { x.Id, x.RoleName }));
                        changed.Append("新增角色");
                        changed.Append($"[{ addRoleNames}]");
                    }

                    if (deleteRoles != null && deleteRoles.Count > 0)
                    {
                        var deleteSysRoles = _sysRoleService.GetRoles(deleteRoles);
                        string deleteRoleNames = string.Join(",", deleteSysRoles.Select(x => new { x.Id, x.RoleName }));
                        changed.AppendLine("删除角色");
                        changed.Append($"[{ deleteRoleNames }]");
                    }

                    var optSysUser = SysUserService.GetUserInfoByUserId(optUserId.Value);
                    _operationLogService.AddLog(new OperationLog()
                    {
                        UserId = optSysUser.UserId.ToString(),
                        UserName = optSysUser.NickName,
                        UserType = optSysUser.UserType,
                        BusinessParam = sysUser.UserId.ToString(),
                        LogContent = $"用户{ sysUser.NickName }的角色修改",
                        ChangedFields = changed.ToString(),
                        ResourceType = (int)GlobalEnum.ResourceType.SYSUSER_ROLE_EDIT,
                    });
                }
                return true;
            }
        }
    }
}
