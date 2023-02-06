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
        /// 携程直连操作类型
        /// </summary>
        public enum CtripDirectOperaterType
        {
            [Description("接受（安排）")]
            Confirmed = 0,
            [Description("拒绝")]
            Reject = 1,
            [Description("接受取消")]
            ReceiveCancel = 11,
            [Description("拒绝取消")]
            RejectCancel = 12,
            [Description("确认入住")]
            CheckIn = 20,

        }

        /// <summary>
        /// 携程直连新版订单状态
        /// </summary>
        public enum CtripDirectionNewOrderStatus
        {
            [Description("新订未处理（确认中）")]
            Confirming = 100,
            [Description("接受新订")]
            Confirmed = 101,
            [Description("拒绝新订")]
            Reject = 102,
            [Description("取消未处理（取消中）")]
            Canceling = 200,
            [Description("接受取消")]
            Canceled = 201,
            [Description("拒绝取消")]
            RejectCancel = 202,

        }
        /// <summary>
        /// 携程直连当拒单和取消拒单时的原因类型
        /// </summary>
        public enum CtripDirectRejectType
        {
            [Description("满房")]
            NoRoom = 1,
            [Description("房价不对")]
            PriceError = 2,
            [Description("其他原因")]
            Other = 3,
        }

   
        /// <summary>
        /// 携程直连房态售卖房型上下线
        /// </summary>
        public enum CtripDirectRoomStatus
        {
            [Description("下线")]
            OffLine = 0,
            [Description("上线")]
            OnLine = 1,
            [Description("删除")]
            Delete = 2

        }


        /// <summary>
        /// 携程直连房态 0满房1销售2限量
        /// </summary>
        public enum CtripDirectSaleStatus
        {
            [Description("满房")]
            SoldOut = 0,
            [Description("销售")]
            Sale = 1,
            [Description("限量")]
            Limited = 2

        }




    }
}
