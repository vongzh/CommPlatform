using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Zk.HotelPlatform.DBProvider;
using Zk.HotelPlatform.Model;
using Zk.HotelPlatform.Utils;
using Zk.HotelPlatform.Utils.Global;

namespace Zk.HotelPlatform.Service.Impl
{
    public class SysRolePermissionService : DataService<SysRolePermission>, ISysRolePermissionService
    {
        private ISysRoleService _sysRoleService = null;
        private readonly IOperationLogService _operationLogService = null;
        private readonly ISysMenuService _sysMenuService = null;

        public ISysUserService SysUserService { get; set; }

        public SysRolePermissionService(ISysRoleService sysRoleService, ISysMenuService sysMenuService, IOperationLogService operationLogService, IDataProvider<SysRolePermission> dataProvider)
            :base(dataProvider)
        {
            this._sysRoleService = sysRoleService;
            _operationLogService = operationLogService;
            _sysMenuService = sysMenuService;
        }

        /// <summary>
        /// 查询角色权限
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public IEnumerable<SysRolePermission> GetRolePermissions(int roleId)
        {
            return base.Select(x => x.RoleId == roleId);
        }

        /// <summary>
        /// 查询角色权限
        /// </summary>
        /// <param name="roleIds"></param>
        /// <returns></returns>
        public IEnumerable<SysRolePermission> GetRolePermissions(IEnumerable<int> roleIds)
        {
            return base.Select(x => roleIds.Contains(x.RoleId));
        }

        /// <summary>
        /// 保存角色权限
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="permissionIds"></param>
        /// <returns></returns>
        public bool SaveRolePermission(int roleId, IEnumerable<int> permissionIds, int userId)
        {
            var role = this._sysRoleService.GetRole(roleId);
            if (role == null)
                throw new BusinessException("角色不存在");
            var sysMenus = _sysMenuService.GetMenus();
            if (sysMenus == null || sysMenus.Count() <= 0)
                throw new BusinessException("未找到可分配的菜单");

            var sysMenuIds = sysMenus.Select(x => x.Id);
            permissionIds = permissionIds.Where(x => sysMenuIds.Contains(x));

            var currentRolePermissions = base.Select(x => x.RoleId == roleId);
            if (currentRolePermissions == null || currentRolePermissions.Count() == 0)
            {
                permissionIds.ToList().ForEach(x =>
                {
                    base.AddEntity(new SysRolePermission()
                    {
                        PermissionId = x,
                        RoleId = roleId
                    });
                });

                var addMenus = _sysMenuService.GetMenus(permissionIds);
                string menuNames = string.Join(",", addMenus.Select(x => new { x.Id, x.Name }));

                var optSysUser = SysUserService.GetUserInfoByUserId(userId);
                _operationLogService.AddLog(new OperationLog()
                {
                    UserId = optSysUser.UserId.ToString(),
                    UserName = optSysUser.NickName,
                    UserType = optSysUser.UserType,
                    BusinessParam = roleId.ToString(),
                    LogContent = $"角色{ role.RoleName }的菜单修改",
                    ChangedFields = $"新增菜单[{menuNames}]",
                    ResourceType = (int)GlobalEnum.ResourceType.EDIT_ROLE_MENU,
                });
                return true;
            }
            else
            {
                var currentPermissionIds = currentRolePermissions.Select(x => x.PermissionId);

                //新增
                var addPermissions = permissionIds.Except(currentPermissionIds);
                if (addPermissions != null && addPermissions.Any())
                {
                    addPermissions.ToList().ForEach(x =>
                    {
                        base.AddEntity(new SysRolePermission()
                        {
                            PermissionId = x,
                            RoleId = roleId
                        });
                    });
                }

                //删除
                var delPermissions = currentPermissionIds.Except(permissionIds);
                if (delPermissions != null && delPermissions.Any())
                {
                    base.DeleteAll(x => x.RoleId == roleId && delPermissions.Contains(x.PermissionId));
                }

                StringBuilder changed = new StringBuilder();

                if (addPermissions != null && addPermissions.Count() > 0)
                {
                    var addMenus = _sysMenuService.GetMenus(addPermissions);
                    string addMenuNames = string.Join(",", addMenus.Select(x => new { x.Id, x.Name }));
                    changed.Append("新增菜单");
                    changed.Append($"[{ addMenuNames}]");
                }

                if (delPermissions != null && delPermissions.Count() > 0)
                {
                    var deleteMenus = _sysMenuService.GetMenus(delPermissions);
                    string deleteMenuNames = string.Join(",", deleteMenus.Select(x => new { x.Id, x.Name }));
                    changed.AppendLine("删除菜单");
                    changed.Append($"[{ deleteMenuNames }]");
                }

                var optSysUser = SysUserService.GetUserInfoByUserId(userId);
                _operationLogService.AddLog(new OperationLog()
                {
                    UserId = optSysUser.UserId.ToString(),
                    UserName = optSysUser.NickName,
                    UserType = optSysUser.UserType,
                    BusinessParam = roleId.ToString(),
                    LogContent = $"角色{ role.RoleName }的菜单修改",
                    ChangedFields = changed.ToString(),
                    ResourceType = (int)GlobalEnum.ResourceType.EDIT_ROLE_MENU,
                });

                return true;
            }
        }
    }
}
