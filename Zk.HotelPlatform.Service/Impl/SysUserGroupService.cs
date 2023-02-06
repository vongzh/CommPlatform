using Aop.Api.Domain;
using AutoMapper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Zk.HotelPlatform.DBProvider;
using Zk.HotelPlatform.Model;
using Zk.HotelPlatform.Model.Request;
using Zk.HotelPlatform.Model.Response;
using Zk.HotelPlatform.Utils;
using Zk.HotelPlatform.Utils.Global;

namespace Zk.HotelPlatform.Service.Impl
{
    public class SysUserGroupService : DataService<SysUserGroup>, ISysUserGroupService
    {
        private IMapper _mapper = null;
        private readonly IOperationLogService _operationLogService = null;

        public ISysUserService SysUserService { get; set; }

        public SysUserGroupService(IMapper mapper, IDataProvider<SysUserGroup> dataProvider, IOperationLogService operationLogService) : base(dataProvider)
        {
            this._mapper = mapper;
            _operationLogService = operationLogService;
        }

        public SysUserGroupResponse GetUserGroup(int id)
        {
            Expression<Func<SysUserGroup, bool>> filter = x => x.Id == id && x.IsDelete == (int)GlobalEnum.YESOrNO.N;
            var sysUserGroup = base.Get(filter);
            return _mapper.Map<SysUserGroup, SysUserGroupResponse>(sysUserGroup);
        }

        public SysUserGroupResponse GetUserGroup(string meta)
        {
            Expression<Func<SysUserGroup, bool>> filter = x => x.Meta == meta && x.IsDelete == (int)GlobalEnum.YESOrNO.N;
            var sysUserGroup = base.Get(filter);
            return _mapper.Map<SysUserGroup, SysUserGroupResponse>(sysUserGroup);
        }

        public IEnumerable<SysUserGroupResponse> GetUserGroups()
        {
            var sysUserGroups = base.Select(x => x.IsDelete == (int)GlobalEnum.YESOrNO.N);
            return _mapper.Map<IEnumerable<SysUserGroup>, IEnumerable<SysUserGroupResponse>>(sysUserGroups);
        }

        public IEnumerable<SysUserGroupResponse> GetUserGroups(SysUserGroupQueryRequest queryRequest)
        {
            Expression<Func<SysUserGroup, bool>> filter = x => x.IsDelete == (int)GlobalEnum.YESOrNO.N;

            if (!string.IsNullOrWhiteSpace(queryRequest.GroupName))
            {
                filter = filter.And(x => x.GroupName.Contains(queryRequest.GroupName));
            }
            if (!string.IsNullOrWhiteSpace(queryRequest.Meta))
            {
                filter = filter.And(x => x.Meta.Contains(queryRequest.Meta));
            }

            var sysUserGroups = base.Select(filter);
            if (sysUserGroups == null)
                return new List<SysUserGroupResponse>();

            return _mapper.Map<IEnumerable<SysUserGroup>, IEnumerable<SysUserGroupResponse>>(sysUserGroups);
        }

        public bool SaveUserGroup(SysUserGroupAddRequest addRequest, int userId)
        {
            var sysUserGroup = _mapper.Map<SysUserGroupAddRequest, SysUserGroup>(addRequest);
            if (string.IsNullOrWhiteSpace(sysUserGroup.GroupName))
                throw new BusinessException("用户组名称不能为空");

            if (sysUserGroup.Id <= 0)
            {
                var exist = base.Get(x => x.Meta == sysUserGroup.Meta && x.IsDelete == (int)GlobalEnum.YESOrNO.N);
                if (exist != null)
                    throw new BusinessException("用户组已存在");

                sysUserGroup.CreateTime = DateTime.Now;
                var entity = base.AddAndGetEntity(sysUserGroup);
                var ret = entity != null && entity.Id > 0;
                if (ret)
                {
                    var optSysUser = SysUserService.GetUserInfoByUserId(userId);
                    _operationLogService.AddLog(new OperationLog()
                    {
                        UserId = optSysUser.UserId.ToString(),
                        UserName = optSysUser.NickName,
                        UserType = optSysUser.UserType,
                        BusinessParam = entity.Id.ToString(),
                        LogContent = $"新增用户组{ entity.GroupName }",
                        ChangedFields = JsonConvert.SerializeObject(new
                        {
                            entity.Id,
                            entity.GroupName
                        }),
                        ResourceType = (int)GlobalEnum.ResourceType.NEW_USERGROUP,
                    });
                }
                return ret;
            }
            else
            {
                var exist = base.Get(x => x.Meta == sysUserGroup.Meta && x.IsDelete == (int)GlobalEnum.YESOrNO.N);
                if (exist != null)
                {
                    if (exist.Id != sysUserGroup.Id)
                    {
                        throw new BusinessException("用户组已存在");
                    }
                }

                var editGroup = base.Get(x => x.Id == sysUserGroup.Id);
                var tempGroup = editGroup.DeepClone();

                editGroup.GroupName = sysUserGroup.GroupName;
                editGroup.Meta = sysUserGroup.Meta;
                editGroup.Description = sysUserGroup.Description;
                editGroup.UpdateTime = DateTime.Now;
                var ret = base.Update(editGroup);
                if (ret)
                {
                    var optSysUser = SysUserService.GetUserInfoByUserId(userId);
                    _operationLogService.AddLog(new OperationLog()
                    {
                        UserId = optSysUser.UserId.ToString(),
                        UserName = optSysUser.NickName,
                        UserType = optSysUser.UserType,
                        BusinessParam = editGroup.Id.ToString(),
                        LogContent = $"修改用户组{ editGroup.GroupName }",
                        ChangedFields = tempGroup.GetChangedFields(editGroup),
                        ResourceType = (int)GlobalEnum.ResourceType.EDIT_USERGROUP,
                    });
                }
                return ret;
            }
        }

        public bool Delete(int id, int userId)
        {
            var sysUserGroup = base.Get(x => x.Id == id && x.IsDelete == (int)GlobalEnum.YESOrNO.N);
            if (sysUserGroup == null)
                throw new BusinessException("用户组不存在");

            sysUserGroup.IsDelete = (int)GlobalEnum.YESOrNO.Y;
            var ret = base.Update(sysUserGroup);
            if (ret)
            {
                var optSysUser = SysUserService.GetUserInfoByUserId(userId);
                _operationLogService.AddLog(new OperationLog()
                {
                    UserId = optSysUser.UserId.ToString(),
                    UserName = optSysUser.NickName,
                    UserType = optSysUser.UserType,
                    BusinessParam = sysUserGroup.Id.ToString(),
                    LogContent = $"删除用户组{ sysUserGroup.GroupName }",
                    ChangedFields = JsonConvert.SerializeObject(new
                    {
                        sysUserGroup.Id,
                        sysUserGroup.GroupName
                    }),
                    ResourceType = (int)GlobalEnum.ResourceType.DELETE_USERGROUP,
                });

            }
            return ret;
        }

        public IEnumerable<SysUserGroupResponse> GetUserGroups(IEnumerable<int> ids)
        {
            var sysUserGroups = base.Select(x => ids.Contains(x.Id) && x.IsDelete == (int)GlobalEnum.YESOrNO.N);
            return _mapper.Map<IEnumerable<SysUserGroup>, IEnumerable<SysUserGroupResponse>>(sysUserGroups);
        }
    }
}
