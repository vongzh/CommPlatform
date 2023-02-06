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
    public class SysUserGroupUserService : DataService<SysUserGroupUser>, ISysUserGroupUserService
    {
        private readonly IOperationLogService _operationLogService = null;
        private readonly ISysUserGroupService _sysUserGroupService = null;

        public ISysUserService SysUserService { get; set; }

        public SysUserGroupUserService(IDataProvider<SysUserGroupUser> dataProvider, ISysUserGroupService sysUserGroupService, IOperationLogService operationLogService)
            : base(dataProvider)
        {
            _operationLogService = operationLogService;
            _sysUserGroupService = sysUserGroupService;
        }

        public IEnumerable<SysUserGroupUser> GetSysUserGroupUsers(int groupId)
        {
            return base.Select(x => x.UserGroupId == groupId);
        }

        public IEnumerable<SysUserGroupUser> GetUserGroups(int uid)
        {
            return base.Select(x => x.UserId == uid);
        }

        public bool SaveUserGroups(int userId, IEnumerable<int> groupIds, int? optUserId = null)
        {
            var sysUser = SysUserService.GetUserInfoByUserId(userId);
            if (sysUser == null)
                throw new BusinessException("未找到用户信息");

            var userGroups = base.Select(x => x.UserId == sysUser.Id).ToList();
            if (userGroups == null || userGroups.Count <= 0)
            {
                groupIds.ToList().ForEach(groupId =>
                {
                    base.AddEntity(new SysUserGroupUser()
                    {
                        UserGroupId = groupId,
                        UserId = sysUser.Id
                    });
                });

                var addUserGroups = _sysUserGroupService.GetUserGroups(groupIds);
                string groupNames = string.Join(",", addUserGroups.Select(x => new { x.Id, x.GroupName }));

                if (optUserId.HasValue)
                {
                    var optSysUser = SysUserService.GetUserInfoByUserId(optUserId.Value);
                    _operationLogService.AddLog(new OperationLog()
                    {
                        UserId = optSysUser.UserId.ToString(),
                        UserName = optSysUser.NickName,
                        UserType = optSysUser.UserType,
                        BusinessParam = sysUser.UserId.ToString(),
                        LogContent = $"用户{ sysUser.NickName }的用户组修改",
                        ChangedFields = $"新增用户组[{groupNames}]",
                        ResourceType = (int)GlobalEnum.ResourceType.SYSUSER_USERGROUP_EDIT,
                    });
                }
                return true;
            }
            else
            {
                var groupUserIds = userGroups.Select(x => x.UserGroupId);

                var addUserGroups = groupIds.Except(groupUserIds).ToList();
                if (addUserGroups != null && addUserGroups.Any())
                {
                    addUserGroups.ForEach(groupId =>
                    {
                        base.AddEntity(new SysUserGroupUser()
                        {
                            UserId = sysUser.Id,
                            UserGroupId = groupId
                        });
                    });
                }

                var deleteUserGroups = groupUserIds.Except(groupIds).ToList();
                if (deleteUserGroups != null && deleteUserGroups.Any())
                {
                    base.DeleteAll(z => z.UserId == sysUser.Id && deleteUserGroups.Contains(z.UserGroupId));
                }

                StringBuilder changed = new StringBuilder();

                if (addUserGroups != null && addUserGroups.Count > 0)
                {
                    var addSysUserGroups = _sysUserGroupService.GetUserGroups(addUserGroups);
                    string addGroupNames = string.Join(",", addSysUserGroups.Select(x => new { x.Id, x.GroupName }));
                    changed.Append("新增用户组");
                    changed.Append($"[{ addGroupNames}]");
                }

                if (deleteUserGroups != null && deleteUserGroups.Count > 0)
                {
                    var deleteSysUserGroups = _sysUserGroupService.GetUserGroups(deleteUserGroups);
                    string deleteGroupNames = string.Join(",", deleteSysUserGroups.Select(x => new { x.Id, x.GroupName }));
                    changed.AppendLine("删除用户组");
                    changed.Append($"[{ deleteGroupNames }]");
                }

                if (optUserId.HasValue)
                {
                    var optSysUser = SysUserService.GetUserInfoByUserId(optUserId.Value);
                    _operationLogService.AddLog(new OperationLog()
                    {
                        UserId = optSysUser.UserId.ToString(),
                        UserName = optSysUser.NickName,
                        UserType = optSysUser.UserType,
                        BusinessParam = sysUser.UserId.ToString(),
                        LogContent = $"用户{ sysUser.NickName }的用户组修改",
                        ChangedFields = changed.ToString(),
                        ResourceType = (int)GlobalEnum.ResourceType.SYSUSER_USERGROUP_EDIT,
                    });
                }
                return true;
            }
        }

        public bool SaveSysUserGroupUsers(int groupId, IEnumerable<int> sysUserGroupUsers, int userId)
        {
            var userGroup = _sysUserGroupService.GetUserGroup(groupId);
            if (userGroup == null)
                throw new BusinessException("未找到用户组");

            var currentGroupUsers = base.Select(x => x.UserGroupId == groupId).ToList();
            if (currentGroupUsers == null || currentGroupUsers.Count <= 0)
            {
                sysUserGroupUsers.ToList().ForEach(groupUser =>
                {
                    base.AddEntity(new SysUserGroupUser()
                    {
                        UserGroupId = groupId,
                        UserId = groupUser
                    });
                });

                var addSysUsers = SysUserService.Select(x => sysUserGroupUsers.Contains(x.Id)).Select(x => new { x.UserId, x.NickName });
                var users = string.Join(",", addSysUsers);
                
                var optSysUser = SysUserService.GetUserInfoByUserId(userId);
                _operationLogService.AddLog(new OperationLog()
                {
                    UserId = optSysUser.UserId.ToString(),
                    UserName = optSysUser.NickName,
                    UserType = optSysUser.UserType,
                    BusinessParam = groupId.ToString(),
                    LogContent = $"用户组{ userGroup.GroupName }的用户修改",
                    ChangedFields = $"新增用户[{users}]",
                    ResourceType = (int)GlobalEnum.ResourceType.USERGROUP_USER__EDIT,
                });

                return true;
            }
            else
            {
                var groupUserIds = currentGroupUsers.Select(x => x.UserId);

                var addGroupUsers = sysUserGroupUsers.Except(groupUserIds).ToList();
                if (addGroupUsers != null && addGroupUsers.Any())
                {
                    addGroupUsers.ForEach(groupUser =>
                    {
                        base.AddEntity(new SysUserGroupUser()
                        {
                            UserId = groupUser,
                            UserGroupId = groupId
                        });
                    });
                }

                var deleteGroupUsers = groupUserIds.Except(sysUserGroupUsers).ToList();
                if (deleteGroupUsers != null && deleteGroupUsers.Any())
                {
                    base.DeleteAll(z => z.UserGroupId == groupId && deleteGroupUsers.Contains(z.UserId));
                }

                StringBuilder changed = new StringBuilder();

                if (addGroupUsers != null && addGroupUsers.Count > 0)
                {
                    var addSysUsers = SysUserService.Select(x => addGroupUsers.Contains(x.Id))
                        .Select(x => new { x.UserId, x.NickName });
                    var addUsers = string.Join(",", addSysUsers);
                    changed.Append("新增用户");
                    changed.Append($"[{ addUsers}]");
                }

                if (deleteGroupUsers != null && deleteGroupUsers.Count > 0)
                {
                    var addSysUsers = SysUserService.Select(x => deleteGroupUsers.Contains(x.Id))
                       .Select(x => new { x.UserId, x.NickName });
                    var deleteUsers = string.Join(",", addSysUsers);
                    changed.Append("删除用户");
                    changed.Append($"[{ deleteUsers }]");
                }

                var optSysUser = SysUserService.GetUserInfoByUserId(userId);
                _operationLogService.AddLog(new OperationLog()
                {
                    UserId = optSysUser.UserId.ToString(),
                    UserName = optSysUser.NickName,
                    UserType = optSysUser.UserType,
                    BusinessParam = groupId.ToString(),
                    LogContent = $"用户组{ userGroup.GroupName }的用户修改",
                    ChangedFields = changed.ToString(),
                    ResourceType = (int)GlobalEnum.ResourceType.USERGROUP_USER__EDIT,
                });

                return true;
            }
        }
    }
}
