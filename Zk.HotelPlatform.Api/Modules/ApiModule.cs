using Autofac;
using AutoMapper;
using Microsoft.AspNet.SignalR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Owin.Security.OAuth;
using Zk.HotelPlatform.Api.Profiles;
using Zk.HotelPlatform.Api.Providers;
using Zk.HotelPlatform.Api.Providers.Impl;

namespace Zk.HotelPlatform.Api.Modules
{
    public class ApiModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register<IConfigurationProvider>(ctx => new MapperConfiguration(cfg =>
                cfg.AddProfile(new MapperProfile())
            )).SingleInstance();

            builder.Register<IMemoryCache>(opt => new MemoryCache(new MemoryCacheOptions())).SingleInstance();

            builder.Register<IMapper>(ctx => new Mapper(ctx.Resolve<IConfigurationProvider>(), ctx.Resolve)).SingleInstance();

            //OAuth
            builder.RegisterType<AuthService>().As<IAuthService>().SingleInstance();
            builder.RegisterType<OAuthServerProvider>().As<IOAuthAuthorizationServerProvider>().SingleInstance();

            //SignalR
            builder.RegisterType<NotificationsUserProvider>().As<IUserIdProvider>().SingleInstance();
        }
    }
}
