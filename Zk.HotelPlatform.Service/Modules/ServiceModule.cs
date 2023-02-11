using Autofac;
using WebApiThrottle;
using Zk.HotelPlatform.Model;
using Zk.HotelPlatform.Service.Impl;

namespace Zk.HotelPlatform.Service.Modules
{
    public class ServiceModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {

            builder.RegisterType<CourseService>().As<ICourseService>().SingleInstance();

            builder.RegisterType<SchemeService>().As<ISchemeService>().SingleInstance();
            builder.RegisterType<ThrottleService>().As<IThrottleService>().SingleInstance();

            builder.RegisterType<OperationLogService>().As<IOperationLogService>().SingleInstance();
            builder.RegisterType<LoginLogService>().As<ILoginLogService>().SingleInstance();
            builder.RegisterType<CaptchaService>().As<ICaptchaService>().SingleInstance();

            builder.RegisterType<SysConfigService>().As<ISysConfigService>().SingleInstance()
               .PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies);

            // 系统
            builder.RegisterType<MainService>().As<IMainService>().SingleInstance();
            builder.RegisterType<SysMenuService>().As<ISysMenuService>().SingleInstance()
                .PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies);
            builder.RegisterType<SysUserService>().As<ISysUserService>().SingleInstance()
                 .PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies);
            builder.RegisterType<SysRoleService>().As<ISysRoleService>().SingleInstance()
                .PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies);

            builder.RegisterType<SysUserIPService>().As<ISysUserIPService>().SingleInstance();

            builder.RegisterType<SysRolePermissionService>().As<ISysRolePermissionService>().SingleInstance()
                .PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies);
            builder.RegisterType<SysDepartmentService>().As<ISysDepartmentService>().SingleInstance()
             .PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies);
            builder.RegisterType<SysUserRoleService>().As<ISysUserRoleService>().SingleInstance()
                .PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies);
            builder.RegisterType<SysUserGroupService>().As<ISysUserGroupService>().SingleInstance()
                .PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies);
            builder.RegisterType<SysUserGroupUserService>().As<ISysUserGroupUserService>().SingleInstance()
                .PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies);
            builder.RegisterType<SysModuleService>().As<ISysModuleService>().SingleInstance()
                 .PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies);
            builder.RegisterType<SysUserGroupModuleService>().As<ISysUserGroupModuleService>().SingleInstance()
                .PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies);
            builder.RegisterType<MessageService>().As<IMessageService>().SingleInstance().PropertiesAutowired();
            
            builder.RegisterType<PolicyRepository>().As<IPolicyRepository>().SingleInstance();
            builder.RegisterType<ThrottleRepository>().As<IThrottleRepository>().SingleInstance();
            builder.RegisterType<ThrottleLogger>().As<IThrottleLogger>().SingleInstance();

        
            //微信
            builder.RegisterType<WeChatService>().As<IWeChatService>().SingleInstance()
                .PropertiesAutowired();

            builder.RegisterGeneric(typeof(DataService<>)).As(typeof(IDataService<>)).SingleInstance()
                .PropertiesAutowired();

            //SignalR
            builder.RegisterType<MessageHub>().SingleInstance();
        }
    }
}
