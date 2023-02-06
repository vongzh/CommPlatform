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
using Zk.HotelPlatform.Model.Basic.Pager;
using Zk.HotelPlatform.Model.Request;
using Zk.HotelPlatform.Model.Response;
using Zk.HotelPlatform.Utils;
using Zk.HotelPlatform.Utils.Global;

namespace Zk.HotelPlatform.Service.Impl
{
    public class SysModuleService : DataService<SysModule>, ISysModuleService
    {
        private IMapper _mapper = null;
        private readonly IOperationLogService _operationLogService = null;

        public ISysUserService SysUserService { get; set; }

        public SysModuleService(IMapper mapper, IDataProvider<SysModule> dataProvider, IOperationLogService operationLogService)
            : base(dataProvider)
        {
            this._mapper = mapper;
            _operationLogService = operationLogService;
        }

        public SysModuleResponse GetModule(int id)
        {
            Expression<Func<SysModule, bool>> filter = x => x.Id == id && x.IsDelete == (int)GlobalEnum.YESOrNO.N;
            var sysModule = base.Get(filter);
            return _mapper.Map<SysModule, SysModuleResponse>(sysModule);
        }

        public IEnumerable<SysModule> GetSysModules()
        {
            var sysModules = base.Select(x => x.IsDelete == (int)GlobalEnum.YESOrNO.N);
            return sysModules;
        }

        public IEnumerable<SysModuleResponse> GetSysModules(IEnumerable<int> moduleIds)
        {
            var sysModules = base.Select(x => moduleIds.Contains(x.Id) && x.IsDelete == (int)GlobalEnum.YESOrNO.N);
            return _mapper.Map<IEnumerable<SysModule>, IEnumerable<SysModuleResponse>>(sysModules);
        }

        public IEnumerable<SysModuleResponse> GetSysModules(SysModuleQueryRequest queryRequest)
        {
            Expression<Func<SysModule, bool>> filter = x => x.IsDelete == (int)GlobalEnum.YESOrNO.N;

            if (!string.IsNullOrWhiteSpace(queryRequest.ModuleName))
            {
                filter = filter.And(x => x.ModuleName.Contains(queryRequest.ModuleName));
            }
            if (!string.IsNullOrWhiteSpace(queryRequest.Meta))
            {
                filter = filter.And(x => x.Meta.Contains(queryRequest.Meta));
            }

            //IOrderedQueryable<SysModule> orderby(IQueryable<SysModule> x) => x.OrderByDescending(z => z.CreateTime).ThenByDescending(y => y.Id);
            //var sysModules = base.Select(out int totalCount, filter, queryRequest.PageIndex, queryRequest.PageSize, orderby)
            var sysModules = base.Select(filter);
            if (sysModules == null)
            {
                return new List<SysModuleResponse>();
            }

            var sysModuleResponses = _mapper.Map<IEnumerable<SysModule>, IEnumerable<SysModuleResponse>>(sysModules);
            foreach (var sysModuleResponse in sysModuleResponses)
            {
                sysModuleResponse.TypeDesc = GlobalEnum.GetEnumDescription((GlobalEnum.ModuleType)sysModuleResponse.Type);
            }
            return sysModuleResponses;
        }

        public bool SaveModule(SysModuleAddRequest addRequest, int userId)
        {
            var sysModule = _mapper.Map<SysModuleAddRequest, SysModule>(addRequest);
            if (string.IsNullOrWhiteSpace(sysModule.ModuleName))
                throw new BusinessException("模块名称不能为空");
            if (string.IsNullOrWhiteSpace(sysModule.Meta))
                throw new BusinessException("模块标记不能为空");

            if (sysModule.Id <= 0)
            {
                var exist = base.Get(x => x.Meta == sysModule.Meta && x.IsDelete == (int)GlobalEnum.YESOrNO.N);
                if (exist != null)
                {
                    throw new BusinessException("模块已存在");
                }

                sysModule.CreateTime = DateTime.Now;
                var entity = base.AddAndGetEntity(sysModule);
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
                        LogContent = $"新增模块{ entity.ModuleName }",
                        ChangedFields = JsonConvert.SerializeObject(new
                        {
                            entity.Id,
                            entity.ModuleName
                        }),
                        ResourceType = (int)GlobalEnum.ResourceType.NEW_MODULE,
                    });
                }
                return ret;
            }
            else
            {
                var exist = base.Get(x => x.Meta == sysModule.Meta && x.IsDelete == (int)GlobalEnum.YESOrNO.N);
                if (exist != null)
                {
                    if (exist.Id != sysModule.Id)
                    {
                        throw new BusinessException("模块已存在");
                    }
                }

                var editModule = base.Get(x => x.Id == sysModule.Id);
                var tempModule = editModule.DeepClone();

                editModule.Meta = sysModule.Meta;
                editModule.Path = sysModule.Path;
                editModule.Type = sysModule.Type;
                editModule.Control = sysModule.Control;
                editModule.ModuleName = sysModule.ModuleName;
                editModule.Description = sysModule.Description;
                editModule.UpdateTime = DateTime.Now;
                var ret = base.Update(editModule);
                if (ret)
                {
                    var optSysUser = SysUserService.GetUserInfoByUserId(userId);
                    _operationLogService.AddLog(new OperationLog()
                    {
                        UserId = optSysUser.UserId.ToString(),
                        UserName = optSysUser.NickName,
                        UserType = optSysUser.UserType,
                        BusinessParam = editModule.Id.ToString(),
                        LogContent = $"修改模块{ editModule.ModuleName }",
                        ChangedFields = tempModule.GetChangedFields(editModule),
                        ResourceType = (int)GlobalEnum.ResourceType.EDIT_MODULE,
                    });
                }
                return ret;
            }
        }

        public bool Delete(int id, int userId)
        {
            var sysModule = base.Get(x => x.Id == id && x.IsDelete == (int)GlobalEnum.YESOrNO.N);
            if (sysModule == null)
                throw new BusinessException("模块不存在");

            sysModule.IsDelete = (int)GlobalEnum.YESOrNO.Y;
            var ret = base.Update(sysModule);
            if (ret)
            {
                var optSysUser = SysUserService.GetUserInfoByUserId(userId);
                _operationLogService.AddLog(new OperationLog()
                {
                    UserId = optSysUser.UserId.ToString(),
                    UserName = optSysUser.NickName,
                    UserType = optSysUser.UserType,
                    BusinessParam = sysModule.Id.ToString(),
                    LogContent = $"删除模块{ sysModule.ModuleName }",
                    ChangedFields = JsonConvert.SerializeObject(new
                    {
                        sysModule.Id,
                        sysModule.ModuleName
                    }),
                    ResourceType = (int)GlobalEnum.ResourceType.DELETE_MENU,
                });
            }
            return ret;
        }
    }
}
