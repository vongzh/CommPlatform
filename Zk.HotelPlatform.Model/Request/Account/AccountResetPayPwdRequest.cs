using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zk.HotelPlatform.Model.Request.Account
{
    public class AccountResetPayPwdRequest
    {
        public int UserId { get; set; }
        public int OperUserId { get; set; }
        public int OperUserType { get; set; }
    }
}
