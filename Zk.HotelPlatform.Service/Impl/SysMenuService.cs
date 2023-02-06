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
    public class SysMenuService : DataService<SysMenu>, ISysMenuService
    {
        private IMapper _mapper = null;
        private readonly IOperationLogService _operationLogService = null;

        public ISysUserService SysUserService { get; set; }

        public SysMenuService(IMapper mapper,IDataProvider<SysMenu> dataProvider, IOperationLogService operationLogService)
            :base(dataProvider)
        {
            this._mapper = mapper;
            _operationLogService = operationLogService;
        }

        /// <summary>
        /// 查询菜单
        /// </summary>
        /// <returns></returns>
        public IEnumerable<SysMenuResponse> GetMenus(IEnumerable<int> menuIds)
        {
            var sysMenus = base.Select(x => x.IsDelete == (int)GlobalEnum.YESOrNO.N && menuIds.Contains(x.Id));
            return _mapper.Map<IEnumerable<SysMenu>, IEnumerable<SysMenuResponse>>(sysMenus);
        }

        /// <summary>
        /// 查询菜单
        /// </summary>
        /// <returns></returns>
        public IEnumerable<SysMenu> GetMenus()
        {
            return base.Select(x => x.IsDelete == (int)GlobalEnum.YESOrNO.N);
        }

        /// <summary>
        /// 查询菜单
        /// </summary>
        /// <param name="queryRequest"></param>
        /// <returns></returns>
        public IEnumerable<SysMenuResponse> GetMenus(SysMenuQueryRequest queryRequest)
        {
            Expression<Func<SysMenu, bool>> filter = x => x.IsDelete == (int)GlobalEnum.YESOrNO.N;

            if (!string.IsNullOrWhiteSpace(queryRequest.Title))
            {
                filter = filter.And(x => x.Title.Contains(queryRequest.Title));
            }
            if (!string.IsNullOrWhiteSpace(queryRequest.Component))
            {
                filter = filter.And(x => x.Component.Contains(queryRequest.Component));
            }
            if (!string.IsNullOrWhiteSpace(queryRequest.Path))
            {
                filter = filter.And(x => x.Path.Contains(queryRequest.Path));
            }
            if (queryRequest.ParentId.HasValue)
            {
                filter = filter.And(x => x.ParentId == queryRequest.ParentId.Value);
            }
            if (queryRequest.Enabled.HasValue)
            {
                filter = filter.And(x => x.Enabled == queryRequest.Enabled.Value);
            }
            if (queryRequest.Level.HasValue)
            {
                filter = filter.And(x => x.Level == queryRequest.Level.Value);
            }

            var sysMenus = base.Select(filter);
            if (sysMenus == null)
                return new List<SysMenuResponse>();

            var sysMenuResponses = _mapper.Map<IEnumerable<SysMenu>, IEnumerable<SysMenuResponse>>(sysMenus);
            foreach (var sysMenuResponse in sysMenuResponses)
            {
                if (sysMenuResponse.ParentId == 0)
                    continue;

                sysMenuResponse.Parent = sysMenuResponses.FirstOrDefault(x => x.Id == sysMenuResponse.ParentId);
            }
            return sysMenuResponses;
        }

        /// <summary>
        /// 查询菜单级别
        /// </summary>
        /// <returns></returns>
        public IEnumerable<SysMenuResponse> GetNestMenus()
        {
            var sysMenuResponses = this.GetMenus(new SysMenuQueryRequest());
            if (sysMenuResponses == null)
                return new List<SysMenuResponse>();

            //菜单等级
            var level1Menu = sysMenuResponses.Where(x => x.Level == (int)GlobalEnum.MenuLevel.LEVEL1).OrderBy(x=>x.Sort).ToList();
            level1Menu.ForEach(x =>
            {
                var level2Menu = sysMenuResponses.Where(z => z.ParentId == x.Id).OrderBy(z => z.Sort).ToList();
                level2Menu.ForEach(z =>
                {
                    var level3Menu = sysMenuResponses.Where(k => k.ParentId == z.Id).OrderBy(k => k.Sort).ToList();
                    z.SubMenus = level3Menu;
                });
                x.SubMenus = level2Menu;
            });

            return level1Menu;
        }

        /// <summary>
        /// 查询菜单
        /// </summary>
        /// <param name="menuId"></param>
        /// <returns></returns>
        public SysMenuResponse GetMenu(int menuId)
        {
            Expression<Func<SysMenu, bool>> filter = x => x.Id == menuId && x.IsDelete == (int)GlobalEnum.YESOrNO.N;
            var sysMenu = base.Get(filter);
            return _mapper.Map<SysMenu, SysMenuResponse>(sysMenu);
        }

        /// <summary>
        /// 新增菜单
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public bool SaveMenu(SysMenuAddRequest addRequest, int userId)
        {
            if (addRequest.Level != (int)GlobalEnum.MenuLevel.LEVEL1)
            {
                if (addRequest.ParentId <= 0)
                {
                    throw new BusinessException("非一级菜单必须选择父级");
                }
            }

            if (addRequest.Id <= 0)
            {
                var sysMenu = _mapper.Map<SysMenuAddRequest, SysMenu>(addRequest);

                sysMenu.IsDelete = (int)GlobalEnum.YESOrNO.N;
                sysMenu.CreateTime = DateTime.Now;
                var entity = base.AddAndGetEntity(sysMenu);
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
                        LogContent = $"新增菜单{ entity.Name }",
                        ChangedFields = Newtonsoft.Json.JsonConvert.SerializeObject(new
                        {
                            entity.Id,
                            entity.Name
                        }),
                        ResourceType = (int)GlobalEnum.ResourceType.NEW_MENU,
                    });
                }
                return ret;
            }
            else
            {
                var currentSysMenu = base.Get(x => x.Id == addRequest.Id && x.IsDelete == (int)GlobalEnum.YESOrNO.N);
                if (currentSysMenu == null)
                    throw new BusinessException("菜单不存在");

                var tempMenu = currentSysMenu.DeepClone();

                //修改过的属性映射
                currentSysMenu = _mapper.Map(addRequest, currentSysMenu);

                var ret = base.Update(currentSysMenu);
                if (ret)
                {
                    var optSysUser = SysUserService.GetUserInfoByUserId(userId);
                    _operationLogService.AddLog(new OperationLog()
                    {
                        UserId = optSysUser.UserId.ToString(),
                        UserName = optSysUser.NickName,
                        UserType = optSysUser.UserType,
                        BusinessParam = currentSysMenu.Id.ToString(),
                        LogContent = $"修改菜单{ currentSysMenu.Name }",
                        ChangedFields = tempMenu.GetChangedFields(currentSysMenu),
                        ResourceType = (int)GlobalEnum.ResourceType.EDIT_MENU,
                    });
                }
                return ret;
            }
        }

        /// <summary>
        /// 删除菜单
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool Delete(int id,int userId)
        {
            var sysMenu = base.Get(x => x.Id == id && x.IsDelete == (int)GlobalEnum.YESOrNO.N);
            if (sysMenu == null)
                throw new BusinessException("菜单不存在");

            var childrens = GetChildren(id);
            if (childrens != null && childrens.Count() > 0)
            {
                throw new BusinessException("子级菜单未删除");
            }

            sysMenu.IsDelete = (int)GlobalEnum.YESOrNO.Y;
            var ret = base.Update(sysMenu);
            if (ret)
            {
                var optSysUser = SysUserService.GetUserInfoByUserId(userId);
                _operationLogService.AddLog(new OperationLog()
                {
                    UserId = optSysUser.UserId.ToString(),
                    UserName = optSysUser.NickName,
                    UserType = optSysUser.UserType,
                    BusinessParam = sysMenu.Id.ToString(),
                    LogContent = $"删除菜单{ sysMenu.Name }",
                    ChangedFields = JsonConvert.SerializeObject(new
                    {
                        sysMenu.Id,
                        sysMenu.Name
                    }),
                    ResourceType = (int)GlobalEnum.ResourceType.DELETE_MENU,
                });
            }
            return ret;
        }

        /// <summary>
        /// 启用/禁用
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool Enabled(int id, int userId)
        {
            var sysMenu = base.Get(x => x.Id == id && x.IsDelete == (int)GlobalEnum.YESOrNO.N);
            if (sysMenu == null)
                throw new BusinessException("菜单不存在");

            var enabled = (int)GlobalEnum.YESOrNO.Y;
            var disabled = (int)GlobalEnum.YESOrNO.N;
            sysMenu.Enabled = sysMenu.Enabled == enabled ? disabled : enabled;
            var ret = base.Update(sysMenu);
            if (ret)
            {
                var optSysUser = SysUserService.GetUserInfoByUserId(userId);
                _operationLogService.AddLog(new OperationLog()
                {
                    UserId = optSysUser.UserId.ToString(),
                    UserName = optSysUser.NickName,
                    UserType = optSysUser.UserType,
                    BusinessParam = sysMenu.Id.ToString(),
                    LogContent = $"{ (sysMenu.Enabled == disabled ? "禁用" : "启用") }菜单{ sysMenu.Name }",
                    ChangedFields = JsonConvert.SerializeObject(new
                    {
                        sysMenu.Id,
                        sysMenu.Name
                    }),
                    ResourceType = sysMenu.Enabled == disabled ? (int)GlobalEnum.ResourceType.DISABLED_MENU : (int)GlobalEnum.ResourceType.ENDABLED_MENU,
                });
            }
            return ret;
        }

        /// <summary>
        /// 获取子级菜单
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private IEnumerable<SysMenu> GetChildren(int id)
        {
            Expression<Func<SysMenu, bool>> filter = x => x.ParentId == id && x.IsDelete == (int)GlobalEnum.YESOrNO.N;
            return base.Select(filter);
        }
    }
}
