using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zk.HotelPlatform.DBProvider.Base;

namespace Zk.HotelPlatform.DBProvider.Migrations
{
    internal sealed class ConfigurationMigrationsDataBase : DbMigrationsConfiguration<DataContextInitialize>
    {
        public ConfigurationMigrationsDataBase()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
        }

        protected override void Seed(DataContextInitialize context)
        {
            base.Seed(context);
        }
    }
}
