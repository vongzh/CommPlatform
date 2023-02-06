using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zk.HotelPlatform.Utils.Global
{
    public partial class GlobalEnum
    {
        public enum Qunar_OrderStatusCode
        {
            预定成功等待支付 = 1,
            等待房间确认 = 2,
            支付成功待安排房间 = 5,
            担保成功待房间确认 = 29,
            订单取消 = 6,
            退款申请中 = 13
        }

        public enum Qunar_OrderStatus
        {
            CANCELLED = -10,
            NEW_ORDER = 1,
            CONFIRMING = 5,
            CONFIRMED = 10,
            REJECTED = 20,
        }

        public enum Qunar_PayStatus
        {
            PAY_SUCCESS = 10,
            REFUND_SUCCESS = 11
        }

        public enum Qunar_OrderOpt
        {
            ARRANGE_ROOM = 5,
            CONFIRM_ROOM_SUCCESS = 6,
            CONFIRM_ROOM_FAILURE = 7,
            CONFIRM_AND_ARRANGE_ROOM = 8,
        }

        public enum Qunar_ArrangeType
        {
            NAME = 1,
            NUMBER = 2
        }

        public enum Qunar_RoomStatus
        {
            SALE = 0,
            NOSALE = 1
        }

        public enum Qunar_PayType
        {
            PREPAY = 0
        }

        public enum Qunar_ProductType
        {
            STAND = 0,
            FORSALE = 1
        }

        public enum Qunar_Refuse
        {
            CLOSE = 1,
            OVERSALE = 0
        }

        public enum Qunar_RefundType
        {
            NOT = 0,
            CAN = 1
        }

        public enum Qunar_RefundRuleType
        {
            FREE = 0,
            PERSENT = 1
        }
    }
}
