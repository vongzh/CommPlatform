using Autofac;
using Zk.HotelPlatform.CacheProvider.Impl;

namespace Zk.HotelPlatform.CacheProvider.Modules
{
    public class ProviderMoule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<SysUserMemoryCacheProvider>().As<ISysUserCacheProvider>().SingleInstance();
            builder.RegisterType<CaptchaCacheProvider>().As<ICaptchaCacheProvider>().SingleInstance();
            builder.RegisterType<ThrottleCacheProvider>().As<IThrottleCacheProvider>().SingleInstance();
        }
    }
}
