using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zk.HotelPlatform.Utils.Global
{
    public partial class GlobalEnum
    {

        public enum XiaoZhuOrderState
        {
            [Description("初始状态")]
            Init = 0,
            [Description("待确认")]
            WaitConfirm = 1,
            [Description("已确认")]
            Confirmed = 2,
            [Description("拒绝")]
            Reject = 3,
            [Description("超时")]
            Overtime = 4,
            [Description("已取消")]
            Canceled = 5,
            [Description("下单失败")]
            Failed = 6
        }
    }
}
