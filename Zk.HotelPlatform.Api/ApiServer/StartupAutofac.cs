using System;
using Autofac;
using System.Linq;
using System.Web.Http;
using System.Reflection;
using System.Collections.Generic;
using System.IO;
using Autofac.Integration.WebApi;
using Autofac.Integration.SignalR;

namespace Zk.HotelPlatform.Api
{
    public class StartupAutofac : IDisposable
    {
        private static readonly Lazy<StartupAutofac> LazyInstance = new Lazy<StartupAutofac>();
        public static StartupAutofac Instance => LazyInstance.Value;
        public IContainer Container { get; private set; }

        /// <summary>
        /// AutoFac注册
        /// </summary>
        /// <param name="config"></param>
        public void Register(HttpConfiguration config)
        {
            var builder = new ContainerBuilder();
            var assemblies = this.GetAllReferAssemblies();
            var enumerable = assemblies as Assembly[] ?? assemblies.ToArray();
            // Module注册
            builder.RegisterAssemblyModules(enumerable.ToArray());
            // Filter注册
            builder.RegisterWebApiFilterProvider(config);
            // Controller注册
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly()).InstancePerLifetimeScope();
            // Hub注册
            builder.RegisterHubs(Assembly.GetExecutingAssembly()).SingleInstance();

            Container = builder.Build();

            config.DependencyResolver = new AutofacWebApiDependencyResolver(Container);
        }

        private IEnumerable<Assembly> GetAllReferAssemblies()
        {
            var assemblyName = Assembly.GetExecutingAssembly().GetName().Name;
            var appName = assemblyName.Substring(0, assemblyName.LastIndexOf('.'));

            var path = AppDomain.CurrentDomain.BaseDirectory;
            var files = Directory.GetFiles(path, $"{appName}.*", SearchOption.TopDirectoryOnly);
            return files.Where(file =>
                    file.EndsWith(".exe", StringComparison.OrdinalIgnoreCase) ||
                    file.EndsWith(".dll", StringComparison.OrdinalIgnoreCase))
                .Select(Assembly.LoadFile);
        }

        public void Dispose()
        {
            Container?.Dispose();
        }
    }
}
