using Autofac;
using Owin;
using System;
using WebApiThrottle;
using Zk.HotelPlatform.Api.Middleware;
using Zk.HotelPlatform.Service;

namespace Zk.HotelPlatform.Api
{
    public class StartupThrottling
    {
        private static readonly Lazy<StartupThrottling> LazyInstance = new Lazy<StartupThrottling>();
        public static StartupThrottling Instance => LazyInstance.Value;

        public void InitThrottlePolicy(IAppBuilder appBuilder, IContainer container)
        {
            using (var scope = container.BeginLifetimeScope())
            {
                var throttleService = scope.Resolve<IThrottleService>();

                var policy = throttleService.GetThrottlePolicy();
                if (policy == null)
                {
                    policy = new ThrottlePolicy()
                    {
                        ClientThrottling = false,
                        EndpointThrottling = false,
                        IpThrottling = false
                    };
                }
                appBuilder.Use<CustomThrottlingMiddleware>(policy,
                      scope.Resolve<IPolicyRepository>(),
                      scope.Resolve<IThrottleRepository>(),
                      scope.Resolve<IThrottleLogger>(),
                      null);
            }
        }
    }
}
