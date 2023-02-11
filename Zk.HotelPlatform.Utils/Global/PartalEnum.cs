using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zk.HotelPlatform.Utils.Global
{
    public class PartalEnum
    {
        public enum OrderStatus
        {
            [Description("待支付")]
            WAIT_PAY = 0,

            [Description("部分支付")]
            PART_PAY = 5,

            [Description("已完成")]
            COMPLATE = 10
        }
    }
}
