using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zk.HotelPlatform.Model.Basic.Pager;

namespace Zk.HotelPlatform.Model.Request
{
    public class SysUserQueryRequest : PageRequest
    {
        public string Phone { get; set; }
        public int? UserType { get; set; }
        public int? Disabled { get; set; }
        public (DateTime? BeginTime, DateTime? EndTime) RegisterTime { get; set; }
        public string UserName { get; set; }
        public string NickName { get; set; }
    }
}
