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
        /// 美团分销查询价格日历条件类型
        /// </summary>
        public enum MtDistributionQueryPriceConditionType
        {
            [Description("酒店")]
            HotelId = 1,
            [Description("GoodIds")]
            GoodsId = 2
        }

        /// <summary>
        /// 订单状态
        /// </summary>
        public enum MtDistributionOrderResponStatus
        {
            [Description("创建订单 ")]
            create = 10,
            [Description("预定中 ")]
            booking = 20,
            [Description("预定成功 ")]
            book_suc = 21,
            [Description("预定失败 ")]
            book_fail = 22,
            [Description("取消中 ")]
            canceling = 30,
            [Description("取消成功 ")]
            cancel_suc = 31,
            [Description("取消失败 ")]
            cancel_fail = 32,
            [Description("已消费退款(美团客服介入后才可能出现此状态)")]
            abort = 40,
            [Description("已入住 ")]
            book_checkin = 50
        }
    }
}
