using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zk.HotelPlatform.Utils.Global
{
    public class GlobalConst
    {
        public const string CUSTOMERINFORMATION_FLODER = "CustomerInformation";

        public const string SYSUSER_SESSION_CONTEXT = "SYSUSER-SESSION_CONTEXT";
        public const string LOG_CONTEXT = "LOG_CONTEXT";

        public const string REGISTER_USER_ROLE = "RegisterUser";
        public const string CUSTOMER_USER_ROLE = "Customer";
        public const string SUBUSER_USER_ROLE = "SubUser";
        public const string CUSTOMER_USER_USERGROUP = "Customer";

        public const int SYS_PAYMENT_CALLBACK = 10001;
        public const string SYS_PAYMENT_CALLBACK_USERNAME = "系统支付";
        public const int SYS_INVOICE_PRODUCE = 10100;
        public const int SYS_PURCHASE = 10200;
        public const string SYS_PURCHASE_USERNAME = "系统采购";

        public const int SYS__APPLYREFUND = 10400;
        public const string SYS__APPLYREFUNDUSERNAME = "系统申请退款";

        public const int SYS_TASK = 10300;
        public const string SYS_TASK_USERNAME = "系统定时任务";
        public const int SYS_PURCHASE_CANCEL = 10400;
        public const string SYS_PURCHASE_CANCEL_USERNAME = "系统采购单取消";
    }
}
