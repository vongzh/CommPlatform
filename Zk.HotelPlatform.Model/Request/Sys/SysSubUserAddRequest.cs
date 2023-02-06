using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zk.HotelPlatform.Model.Request.Sys
{
    public class SysSubUserAddRequest : BaseSysUser
    {
        /// <summary>
        /// 0启用 1禁用
        /// </summary>
        public int Disabled { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string Pwd { get; set; }
        /// <summary>
        /// 父级用户ID
        /// </summary>
        public int ParentUserId { get; set; }

    }
}
