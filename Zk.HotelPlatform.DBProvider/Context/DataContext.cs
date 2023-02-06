using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zk.HotelPlatform.DBProvider.Migrations;
using Zk.HotelPlatform.Model;
using Zk.HotelPlatform.Utils.Decimal;

namespace Zk.HotelPlatform.DBProvider.Base
{
    public class DataContext : DbContext
    {
        public DataContext() : base("HotelPlatform")
        {
            this.Configuration.LazyLoadingEnabled = true;
            this.Configuration.AutoDetectChangesEnabled = true;
        }

        public DataContext(string conn) : base(conn)
        {
            this.Configuration.LazyLoadingEnabled = true;
            this.Configuration.AutoDetectChangesEnabled = true;
        }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SysUserIP>();

            modelBuilder.Entity<SysConfig>();
            modelBuilder.Entity<SysMenu>();
            modelBuilder.Entity<SysModule>();
            modelBuilder.Entity<SysRole>();
            modelBuilder.Entity<SysRolePermission>();
            modelBuilder.Entity<SysUser>();

            modelBuilder.Entity<LoginLog>();
            modelBuilder.Entity<OperationLog>();

            modelBuilder.Entity<SysUserRole>();
            modelBuilder.Entity<SysUserGroup>();
            modelBuilder.Entity<SysDepartment>();
            modelBuilder.Entity<SysUserGroupModule>();
            modelBuilder.Entity<SysUserGroupUser>();
            modelBuilder.Entity<WeChatUserBind>();
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            modelBuilder.Conventions.Add(new DecimalPrecisionAttributeConvention());
            base.OnModelCreating(modelBuilder);
        }
    }
}
