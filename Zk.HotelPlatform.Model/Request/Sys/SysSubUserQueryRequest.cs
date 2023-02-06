using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zk.HotelPlatform.Model.Basic.Pager;

namespace Zk.HotelPlatform.Model.Request.Sys
{
    public class SysSubUserQueryRequest : PageRequest
    {
        public string UserName { get; set; }
        public string NickName { get; set; }
        public string Mobile { get; set; }
        public int? Disabled { get; set; }
        public int?  ParentUserId { get; set; }
    }

}
