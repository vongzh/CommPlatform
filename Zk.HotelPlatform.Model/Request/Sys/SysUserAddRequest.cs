using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zk.HotelPlatform.Model.Request
{
    public class SysUserAddRequest : BaseSysUser
    {
        /// <summary>
        /// 0主账号  1子账号 默认为主账号
        /// </summary>
        public int IsSubUser { get; set; } = 0;
        public int? CaptchaType { get; set; }
        public string Code { get; set; }

    }
}
