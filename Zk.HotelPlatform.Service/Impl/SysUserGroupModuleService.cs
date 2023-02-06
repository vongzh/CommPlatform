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
    public class SysUserGroupModuleService : DataService<SysUserGroupModule>, ISysUserGroupModuleService
    {
        private readonly IOperationLogService _operationLogService = null;
        private readonly ISysUserGroupService _sysUserGroupService = null;
        private readonly ISysModuleService _sysModuleService = null;

        public ISysUserService SysUserService { get; set; }

        public SysUserGroupModuleService(IDataProvider<SysUserGroupModule> dataProvider, ISysModuleService sysModuleService, ISysUserGroupService sysUserGroupService , IOperationLogService operationLogService)
            : base(dataProvider)
        {
            _sysUserGroupService = sysUserGroupService;
            _sysModuleService = sysModuleService;
            _operationLogService = operationLogService;
        }

        public IEnumerable<SysUserGroupModule> GetUserGroupModules(int groupId)
        {
            return base.Select(x => x.UserGroupId == groupId);
        }

        public IEnumerable<SysUserGroupModule> GetUserGroupModules(IEnumerable<int> groupIds)
        {
            return base.Select(x => groupIds.Contains(x.UserGroupId));
        }

        public bool SaveUserGroupModule(int groupId, IEnumerable<int> groupModules,int userId)
        {
            var userGroup = _sysUserGroupService.GetUserGroup(groupId);
            if (userGroup == null)
                throw new BusinessException("未找到用户组");
            var sysModules = _sysModuleService.GetSysModules();
            if (sysModules == null || sysModules.Count() <= 0)
                throw new BusinessException("未找到可分配的模块");

            // 过滤已删除的模块
            var sysModuleIds = sysModules.Select(x => x.Id);
            groupModules = groupModules.Where(x => sysModuleIds.Contains(x));

            var currentGroupModules = base.Select(x => x.UserGroupId == groupId).ToList();
            if (currentGroupModules == null || currentGroupModules.Count <= 0)
            {
                groupModules.ToList().ForEach(groupUser =>
                {
                    base.AddEntity(new SysUserGroupModule()
                    {
                        UserGroupId = groupId,
                        ModuleId = groupUser
                    });
                });

                var addModules = _sysModuleService.GetSysModules(groupModules);
                string moduleNames = string.Join(",", addModules.Select(x => new { x.Id, x.ModuleName }));

                var optSysUser = SysUserService.GetUserInfoByUserId(userId);
                _operationLogService.AddLog(new OperationLog()
                {
                    UserId = optSysUser.UserId.ToString(),
                    UserName = optSysUser.NickName,
                    UserType = optSysUser.UserType,
                    BusinessParam = groupId.ToString(),
                    LogContent = $"用户组{ userGroup.GroupName }的模块修改",
                    ChangedFields = $"新增模块[{moduleNames}]",
                    ResourceType = (int)GlobalEnum.ResourceType.USERGROUP_MODEULE__EDIT,
                });
                return true;
            }
            else
            {
                var groupModuleIds = currentGroupModules.Select(x => x.ModuleId);

                var addGroupModules = groupModules.Except(groupModuleIds).ToList();
                if (addGroupModules != null && addGroupModules.Any())
                {
                    addGroupModules.ForEach(groupUser =>
                    {
                        base.AddEntity(new SysUserGroupModule()
                        {
                            ModuleId = groupUser,
                            UserGroupId = groupId
                        });
                    });
                }

                var deleteGroupModules = groupModuleIds.Except(groupModules).ToList();
                if (deleteGroupModules != null && deleteGroupModules.Any())
                {
                    base.DeleteAll(z => z.UserGroupId == groupId && deleteGroupModules.Contains(z.ModuleId));
                }

                StringBuilder changed = new StringBuilder();

                if (addGroupModules != null && addGroupModules.Count > 0)
                {
                    var addModules = _sysModuleService.GetSysModules(addGroupModules);
                    string addModuleNames = string.Join(",", addModules.Select(x => new { x.Id, x.ModuleName }));
                    changed.Append("新增模块");
                    changed.Append($"[{ addModuleNames}]");
                }

                if (deleteGroupModules != null && deleteGroupModules.Count > 0)
                {
                    var deleteModules = _sysModuleService.GetSysModules(deleteGroupModules);
                    string deleteModuleNames = string.Join(",", deleteModules.Select(x => new { x.Id, x.ModuleName }));
                    changed.AppendLine("删除模块");
                    changed.Append($"[{ deleteModuleNames }]");
                }

                var optSysUser = SysUserService.GetUserInfoByUserId(userId);
                _operationLogService.AddLog(new OperationLog()
                {
                    UserId = optSysUser.UserId.ToString(),
                    UserName = optSysUser.NickName,
                    UserType = optSysUser.UserType,
                    BusinessParam = groupId.ToString(),
                    LogContent = $"用户组{ userGroup.GroupName }的模块修改",
                    ChangedFields = changed.ToString(),
                    ResourceType = (int)GlobalEnum.ResourceType.USERGROUP_MODEULE__EDIT,
                });

                return true;
            }
        }
    }
}
