using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zk.HotelPlatform.DBProvider.Base;
using Zk.HotelPlatform.DBProvider.Migrations;
using Zk.HotelPlatform.Utils;

namespace Zk.HotelPlatform.DBProvider.Impl
{
    public class DbInitProvider : IDbInitProvider
    {
        private readonly DataContextInitialize _contextInitialize;
        public DbInitProvider(DataContextInitialize contextInitialize)
        {
            _contextInitialize = contextInitialize;
        }

        public void Init()
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<DataContextInitialize, ConfigurationMigrationsDataBase>());

            _contextInitialize.Database.Initialize(true);
        }
    }
}
