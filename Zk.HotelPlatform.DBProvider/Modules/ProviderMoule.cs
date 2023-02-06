using Autofac;
using System.Data.Entity;
using Zk.HotelPlatform.DBProvider.Base;
using Zk.HotelPlatform.DBProvider.Impl;

namespace Zk.HotelPlatform.DBProvider.Modules
{
    public class ProviderMoule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<DataContext>().InstancePerDependency();

            builder.RegisterType<DataContextInitialize>().InstancePerLifetimeScope();

            builder.RegisterGeneric(typeof(DataProvider<>)).As(typeof(IDataProvider<>)).InstancePerLifetimeScope()
                .PropertiesAutowired();

            builder.RegisterType<DbInitProvider>().As<IDbInitProvider>().InstancePerLifetimeScope();
        }
    }
}
