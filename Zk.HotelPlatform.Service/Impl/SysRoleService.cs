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
    public class SysRoleService : DataService<SysRole>, ISysRoleService
    {
        private IMapper _mapper = null;
        private IOperationLogService _operationLogService = null;

        public ISysUserService SysUserService { get; set; }

        public SysRoleService(IMapper mapper, IDataProvider<SysRole> dataProvider, IOperationLogService operationLogService)
            : base(dataProvider)
        {
            this._mapper = mapper;
            _operationLogService = operationLogService;
        }

        /// <summary>
        /// 查询系统角色
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public SysRoleResponse GetRole(int id)
        {
            Expression<Func<SysRole, bool>> filter = x => x.Id == id && x.IsDelete == (int)GlobalEnum.YESOrNO.N;
            var sysRole = base.Get(filter);
            return _mapper.Map<SysRole, SysRoleResponse>(sysRole);
        }

        /// <summary>
        /// 查询角色
        /// </summary>
        /// <param name="meta"></param>
        /// <returns></returns>
        public SysRoleResponse GetRole(string meta)
        {
            Expression<Func<SysRole, bool>> filter = x => x.Meta == meta && x.IsDelete == (int)GlobalEnum.YESOrNO.N;
            var sysRole = base.Get(filter);
            return _mapper.Map<SysRole, SysRoleResponse>(sysRole);
        }

        /// <summary>
        /// 查询系统角色
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IEnumerable<SysRoleResponse> GetRoles(IEnumerable<int> ids)
        {
            Expression<Func<SysRole, bool>> filter = x => ids.Contains(x.Id);
            var sysRoles = base.Select(filter);
            return _mapper.Map<IEnumerable<SysRole>, IEnumerable<SysRoleResponse>>(sysRoles);
        }

        /// <summary>
        /// 查询角色
        /// </summary>
        /// <returns></returns>
        public IEnumerable<SysRoleResponse> GetRoles()
        {
            var roles = base.Select(x => x.IsDelete == (int)GlobalEnum.YESOrNO.N);
            return _mapper.Map<IEnumerable<SysRole>, IEnumerable<SysRoleResponse>>(roles);
        }

        /// <summary>
        /// 查询角色
        /// </summary>
        /// <param name="queryRequest"></param>
        /// <returns></returns>
        public IEnumerable<SysRoleResponse> GetRoles(SysRoleQueryRequest queryRequest)
        {
            Expression<Func<SysRole, bool>> filter = x => x.IsDelete == (int)GlobalEnum.YESOrNO.N;

            if (!string.IsNullOrWhiteSpace(queryRequest.RoleName))
            {
                filter = filter.And(x => x.RoleName.Contains(queryRequest.RoleName));
            }
            if (!string.IsNullOrWhiteSpace(queryRequest.Meta))
            {
                filter = filter.And(x => x.Meta.Contains(queryRequest.Meta));
            }

            var sysRoles = base.Select(filter);
            if (sysRoles == null)
                return new List<SysRoleResponse>();

            return _mapper.Map<IEnumerable<SysRole>, IEnumerable<SysRoleResponse>>(sysRoles);
        }

        /// <summary>
        /// 保存角色
        /// </summary>
        /// <param name="addRequest"></param>
        /// <returns></returns>
        public bool SaveRole(SysRoleAddRequest addRequest, int userId)
        {
            var sysRole = _mapper.Map<SysRoleAddRequest, SysRole>(addRequest);
            if (string.IsNullOrWhiteSpace(sysRole.RoleName))
                throw new BusinessException("角色名称不能为空");
            if (string.IsNullOrWhiteSpace(sysRole.Meta))
                throw new BusinessException("角色标记不能为空");

            if (sysRole.Id <= 0)
            {
                var exist = base.Get(x => x.Meta == sysRole.Meta && x.IsDelete == (int)GlobalEnum.YESOrNO.N);
                if (exist != null)
                    throw new BusinessException("角色标记已存在");

                sysRole.CreateTime = DateTime.Now;
                var entity = base.AddAndGetEntity(sysRole);
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
                        LogContent = $"新增角色{ entity.RoleName }",
                        ChangedFields = JsonConvert.SerializeObject(new
                        {
                            entity.Id,
                            entity.RoleName
                        }),
                        ResourceType = (int)GlobalEnum.ResourceType.NEW_ROLE,
                    });
                }
                return ret;
            }
            else
            {
                var exist = base.Get(x => x.Meta == sysRole.Meta && x.IsDelete == (int)GlobalEnum.YESOrNO.N);
                if (exist != null)
                {
                    if (exist.Id != sysRole.Id)
                    {
                        throw new BusinessException("角色标记已存在");
                    }
                }

                var editRole = base.Get(x => x.Id == sysRole.Id);
                var tempRole = editRole.DeepClone();

                editRole.Meta = sysRole.Meta;
                editRole.RoleName = sysRole.RoleName;
                editRole.Description = sysRole.Description;
                editRole.UpdateTime = DateTime.Now;
                var ret = base.Update(editRole);
                if (ret)
                {
                    var optSysUser = SysUserService.GetUserInfoByUserId(userId);
                    _operationLogService.AddLog(new OperationLog()
                    {
                        UserId = optSysUser.UserId.ToString(),
                        UserName = optSysUser.NickName,
                        UserType = optSysUser.UserType,
                        BusinessParam = editRole.Id.ToString(),
                        LogContent = $"修改角色{ editRole.RoleName }",
                        ChangedFields = tempRole.GetChangedFields(editRole),
                        ResourceType = (int)GlobalEnum.ResourceType.EDIT_ROLE,
                    });
                }
                return ret;
            }
        }

        /// <summary>
        /// 删除角色
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool Delete(int id, int userId)
        {
            var sysRole = base.Get(x => x.Id == id && x.IsDelete == (int)GlobalEnum.YESOrNO.N);
            if (sysRole == null)
                throw new BusinessException("角色不存在");

            sysRole.IsDelete = (int)GlobalEnum.YESOrNO.Y;
            var ret = base.Update(sysRole);
            if (ret)
            {
                var optSysUser = SysUserService.GetUserInfoByUserId(userId);
                _operationLogService.AddLog(new OperationLog()
                {
                    UserId = optSysUser.UserId.ToString(),
                    UserName = optSysUser.NickName,
                    UserType = optSysUser.UserType,
                    BusinessParam = sysRole.Id.ToString(),
                    LogContent = $"删除角色{ sysRole.RoleName }",
                    ChangedFields = 
                    JsonConvert.SerializeObject(new
                    {
                        sysRole.Id,
                        sysRole.RoleName
                    }),
                    ResourceType = (int)GlobalEnum.ResourceType.DELETE_ROLE,
                });
            }
            return ret;
        }
    }
}
