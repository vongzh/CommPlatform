using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zk.HotelPlatform.Model
{
    public class BaseSysUser
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string NickName { get; set; }
        public string Avatar { get; set; }
        public string Mail { get; set; }
        public string Mobile { get; set; }
        public int UserType { get; set; }
        public string Pwd { get; set; }
        public int? UserPlatformId { get; set; }
        public int? ParentUserId { get; set; }

    }
}
