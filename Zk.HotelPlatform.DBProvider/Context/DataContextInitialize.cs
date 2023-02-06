using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zk.HotelPlatform.Utils.Decimal;

namespace Zk.HotelPlatform.DBProvider.Base
{
    public class DataContextInitialize : DbContext
    {
        public DataContextInitialize() : base("HotelPlatform")
        {
            this.Configuration.AutoDetectChangesEnabled = true;
            this.Configuration.LazyLoadingEnabled = true;
        }

        public DataContextInitialize(string conn) : base(conn)
        {
            this.Configuration.AutoDetectChangesEnabled = true;
            this.Configuration.LazyLoadingEnabled = true;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            modelBuilder.Conventions.Add(new DecimalPrecisionAttributeConvention());
            
            base.OnModelCreating(modelBuilder);
        }
    }
}
