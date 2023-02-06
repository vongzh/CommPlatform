using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zk.HotelPlatform.Model;
using Zk.HotelPlatform.Model.Response;

namespace Zk.HotelPlatform.CacheModel
{
    public class SysUserSession : BaseSysUser
    {
        public int UserType { get; set; }
        public int? UserPlatformId { get; set; }
        public string CustomerNo { get; set; }
        public int? Flag { get; set; }
        public int Status { get; set; }
        public string StatusDesc { get; set; }
        public int Disabled { get; set; }
        public string Seed { get; set; }
        public string PrivateKey { get; set; }
        public DateTime Date { get; set; }
        public string ClientIP { get; set; }
        public string Token { get; set; }
        public IEnumerable<SysModuleResponse> Modules { get; set; }
        public IEnumerable<SysRoleResponse> UserRoles { get; set; }
        public IEnumerable<SysMenuResponse> RoleMenus { get; set; }
    }
}
