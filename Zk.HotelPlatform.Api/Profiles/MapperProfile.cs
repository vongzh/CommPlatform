using Zk.HotelPlatform.CacheModel;
using Zk.HotelPlatform.Model;
using Zk.HotelPlatform.Model.Request;
using Zk.HotelPlatform.Model.Response;
using Zk.HotelPlatform.Model.Response.Log;

namespace Zk.HotelPlatform.Api.Profiles
{
    public class MapperProfile : AutoMapper.Profile
    {
        public MapperProfile()
        {
            AllowNullCollections = true;
            AllowNullDestinationValues = true;

            CreateMap<OperationLog, OperationLogResponse>();

           
            CreateMap<SysMenu, SysMenuResponse>();
            CreateMap<SysMenuAddRequest, SysMenu>();

            CreateMap<SysRole, SysRoleResponse>();
            CreateMap<SysRoleAddRequest, SysRole>();

            CreateMap<SysUser, SysUserResponse>()
                 .ForMember(x => x.RegisterTime, y => y.MapFrom(s => s.CreateTime)); ;
            CreateMap<SysUserResponse, SysUser>();
            CreateMap<SysUserAddRequest, SysUser>();
            CreateMap<SysUser, SysUserSession>();
            CreateMap<SysUser, SysUserClient>();
            CreateMap<SysDepartment, SysDepartmentResponse>();

            CreateMap<SysUserGroup, SysUserGroupResponse>();
            CreateMap<SysUserGroupAddRequest, SysUserGroup>();

            CreateMap<SysModule, SysModuleResponse>();
            CreateMap<SysModuleAddRequest, SysModule>();

          
        }
    }
}
