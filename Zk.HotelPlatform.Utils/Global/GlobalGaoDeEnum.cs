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

        /// <summary>
        ///1：待确认
        ///2：预订失败
        ///3：已确认
        ///4：商家拒单
        ///5：订单取消中
        ///6 ：订单已取消
        /// </summary>
        public enum GaoDeOrderStatus
        {
            [Description("待确认")]
            WaitConfirm = 1,
            [Description("预订失败")]
            ReserveFail = 2,
            [Description("已确认")]
            Confirmed = 3,
            [Description("商家拒单")]
            Reject = 4,
            [Description("订单取消中")]
            Canceling = 5,
            [Description("订单已取消")]
            Canceled = 6,

        }

        /// <summary>
        /// 房型状态1:在售 2:下架 -1:失效
        /// </summary>
        public enum GaoDeDirectionRoomStatus
        {
            [Description("在售")]
            Online = 1,
            [Description("下架")]
            SoldOut = 2,
            [Description("失效")]
            Lose = -1,

        }
    }
}
