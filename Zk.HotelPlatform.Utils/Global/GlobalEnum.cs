using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Zk.HotelPlatform.Utils.Global
{
    public partial class GlobalEnum
    {
        public static string GetEnumDescription(Enum enumValue)
        {
            try
            {
                string value = enumValue.ToString();
                FieldInfo field = enumValue.GetType().GetField(value);
                if (field == null)
                {
                    return "未知";
                }
                object[] objs = field.GetCustomAttributes(typeof(DescriptionAttribute), false);
                if (objs.Length == 0)
                    return value;
                DescriptionAttribute descriptionAttribute = (DescriptionAttribute)objs[0];
                return descriptionAttribute.Description;
            }
            catch (Exception)
            {
                return "未知";
            }
        }

        public static int GetEnumValue(Type enumType, string enumName)
        {
            try
            {
                if (!enumType.IsEnum)
                    throw new ArgumentException("enumType必须是枚举类型");
                enumName = enumName.ToUpper();

                var values = Enum.GetValues(enumType);
                var ht = new Hashtable();
                foreach (var val in values)
                {
                    ht.Add(Enum.GetName(enumType, val), val);
                }
                return (int)ht[enumName];
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// 是否
        /// </summary>
        public enum YESOrNO
        {
            [Description("是")]
            Y = 1,
            [Description("否")]
            N = 0
        }

        /// <summary>
        /// 地点类型
        /// </summary>
        public enum PlaceType
        {
            HOT = 0,
            TRAFFIC = 1,
            TRADAREA = 2,
            SUBWAY = 3,
            SCENICAREA = 4,
            UNIVERSITY = 5,
            HOSPITAL = 6,
            BRAND = 7
        }

        /// <summary>
        /// 图片类型
        /// </summary>
        public enum ImgType
        {
            FACADE = 1,
            PUBLICAREA = 2,
            ELSE = 5
        }

        /// <summary>
        /// 注册方式
        /// </summary>
        public enum RegisterType
        {
            NORMAL = 0,
            MOBILE = 1,
            MAIL = 2
        }

        /// <summary>
        /// 用户标识
        /// </summary>
        public enum UserFlag
        {
            REGISTER = 0,
            MOBILEVALID = 2,
            MAILBALID = 4,
            BOTHVALID = 8,
        }

        /// <summary>
        /// 用户状态
        /// </summary>
        public enum UserStatus
        {
            DISABLED = -1,
            NORMAL = 0,
        }

        /// <summary>
        /// 用户类型
        /// </summary>
        public enum UserType
        {
            ALL = 0,
            [Description("接口")]
            API = 200,
            [Description("后台")]
            BACKSTAGE = 100,
            [Description("客户")]
            CUSTOMER = 50,
            [Description("注册用户")]
            USER = 10,
            [Description("系统")]
            SYSTEM = 1000,
            [Description("未知")]
            UNKNOWN = -1
        }
        /// <summary>
        /// 订单状态
        /// </summary>
        public enum OrderStatus
        {
            [Description("订单生成")]
            ORDERCREATE = 1,
            [Description("待支付")]
            WAITPAY = 5,
            [Description("预订中")]
            RESERVEING = 15,
            [Description("待确认")]
            WAITCONFIRM = 20,
            [Description("预订成功")]
            RESERVESUCCESS = 25,
            [Description("预订失败")]
            RESERVEFAIL = 30,
            [Description("已消费")]
            WAITIN = 35,
            [Description("已消费退款")]
            WAITIN_REFUND = 40,
            [Description("已取消")]
            CANCEL = -1,
            [Description("已关闭")]
            CLOSE = -5,
        }

        /// <summary>
        /// 订单取消类型
        /// </summary>
        public enum OrderCancelType
        {
            //付款之前的取消类型：
            [Description("付款前用户取消")]
            USERCANCEL = 1,
            [Description("支付超时系统取消")]
            SYSCANCEL = 2,
            //付款之后取消场景：预订失败状态下 用户取消订单  拒单  待确认状态下 用户可以取消订单
            [Description("预订失败用户取消")]
            RESERVERFAILUSERCANCEL = 3,
            [Description("待确认时用户取消")]
            WATICONFIRMUSERCANCEL = 4
        }

        /// <summary>
        /// 退款状态
        /// </summary>
        public enum RefundStatus
        {
            [Description("待审核")]
            WAITAUDIT = 0,
            //[Description("审核中")]
            //AUDINGING = 1,
            [Description("拒绝退款")]
            REJECT = 2,
            //[Description("审核通过")]
            //AUDITPASS = 3,
            [Description("待退款")]
            WAITREFUND = 4,
            [Description("退款中")]
            REFUNDING = 5,
            [Description("退款成功")]
            REFUNDSUCCESS = 6,
            [Description("退款失败")]
            REFUNDFAIL = 7,
            [Description("关闭")]
            CLOSE = -1
        }

        /// <summary>
        /// 退款类型
        /// </summary>
        public enum RefundType
        {
            //售前
            [Description("用户取消")]
            USERCANCEL = 1,
            [Description("预订失败")]
            RESERVERFAIL = 2,
            [Description("售后退款")]
            AFTERSALE = 3,
            [Description("调账退款")]
            ADJUSTREFUND = 4
            //售后退款类型
            //大类型 小类型
        }
        /// <summary>
        /// 退款路径
        /// </summary>
        public enum RefundWay
        {
            [Description("原路返回")]
            ORIGINAL = 0,
            [Description("线下退款")]
            OFFLINE = 1,
        }

        /// <summary>
        /// 退款原因
        /// </summary>
        public enum RefundReason
        {
            [Description("客户")]
            CUSTOMER = 0,
            [Description("平台")]
            PLATFORM = 1,
            [Description("酒店")]
            HOTEL = 2,
            [Description("系统")]
            System = 10,
            [Description("退款金额误差")]
            RefundAmountError = 11,
            [Description("其它")]
            Else = 100,
        }

        /// <summary>
        /// 订单支付状态
        /// </summary>
        public enum OrderPaymentStatus
        {
            [Description("支付取消")]
            PAYCANCEL = -1,
            [Description("待支付")]
            WAITPAY = 0,
            [Description("支付中")]
            PAYING = 1,
            [Description("支付成功")]
            PAYSUCCESS = 2,
            [Description("支付失败")]
            PAYFAIL = 4,
        }

        /// <summary>
        /// 支付单状态
        /// </summary>
        public enum PaymentOrderStatus
        {
            [Description("支付取消")]
            PAYCANCEL = -1,
            [Description("待支付")]
            WAITPAY = 0,
            [Description("支付中")]
            PAYING = 1,
            [Description("支付成功")]
            PAYSUCCESS = 2,
            [Description("支付失败")]
            PAYFAIL = 4,
        }
        public enum InvoiceDrawer
        {
            [Description("酒店")]
            HOTEL = 0,
        }

        public enum OrderSource
        {
            Web = 5,
            API = 50,
        }

        public enum SysUserStatus
        {
            [Description("锁定")]
            LOCK = -5,
            [Description("正常")]
            NORMAL = 0,
        }

        public enum SysUserDisabled
        {
            [Description("启用")]
            N = 0,
            [Description("禁用")]
            Y = 1,
        }

        public enum PaymentType
        {
            [Description("微信")]
            TENPAY = 1,
            [Description("支付宝")]
            ALIPAY = 5,
            [Description("易宝")]
            YOP = 10,
            [Description("云闪付")]
            UNIONPAY = 20,
            [Description("百度钱包")]
            BAIDUPAY = 30,
            [Description("信用卡")]
            CREDITPAY = 40,
            [Description("余额")]
            BALANCE = 50,
            [Description("接口")]
            APIPAY = 60,
            [Description("银行卡")]
            BANKCARD = 70,
            [Description("优惠券")]
            COUPON = 80,
            [Description("其它")]
            DISCOUNT = 100,
        }

        /// <summary>
        /// 账户余额的支付方式
        /// </summary>
        public enum AccountPaymentType
        {
            [Description("余额支付")]
            Balance = 1,
            [Description("虚拟金额支付")]
            Virtual = 2,
        }

        public enum MenuLevel
        {
            LEVEL1 = 1,
            LEVEL2 = 2,
            LEVEL3 = 3,
        }

        public enum OptFrom
        {
            [Description("系统")]
            SYSTEM = 1,
            [Description("后台用户")]
            BACKSTAGE = 10,
            [Description("用户")]
            CUSTOMER = 20,
            [Description("API用户")]
            API = 30,
        }

        public enum OrderLogOptType
        {

            BOOKING = 1,
            REMIDER = 5,
            PAY = 11,
            CancelOrder = 18,
            REFUND = 20,
            CANCELREFUND = 21,
            REFUNDPASS = 22,
            REFUNDREJECT = 23,
            REFUNDCOMPLATE = 24,
            REFUNDPROCESSING = 25,

            [Description("转人工")]
            TURNTOMANUAL = 26,
            [Description("生成采购单")]
            CREATEPURCHAS = 27,
            [Description("中止采购")]
            SuspendPurchase = 28,
            [Description("重新采购")]
            RePurchase = 29,
            [Description("新建采购单")]
            NEW_PURCHASE_ORDER = 30,
            [Description("采购失败")]
            PURCHASE_FAIL = 31,
            [Description("采购退款")]
            PURCHASE_REFUND = 32,
            [Description("采购取消")]
            PURCHASE_CANCEL = 32,
            [Description("修改采购单")]
            EDIT_PURCHASE_ORDER = 35,
            [Description("待拒单")]
            WAITREJECT = 33,
            INVOICE = 50,
            CONFIRM = 100,
            REJECT = 101,
            OVERTIME = 110,
            LOCK = 120,
            UNLOCK = 121,
            Ctrip_GetOrderStatus = 201,
            Ctrip_OrderSave = 205,
            Ctrip_OrderSubmit = 208,
            Ctrip_OrderBookCheck = 209,
            MTFX_OrderBookCheck = 210,
            Elong_OrderBookCheck = 211,
            JinJiang_OrderBookCheck = 222,
            FZFX_OrderBookCheck = 217,
            MTFX_OrderSubmit = 212,
            Ctrip_PurchaseSucess = 213,
            Manual_PurchaseSucess = 214,
            Ctrip_PurchaseFail = 215,
            [Description("确认成功")]
            ConfirmSuccess = 218,
            ConfirmFail = 216,
            CheckIn = 220,
            AFTERSALE_HANDLER = 250,
            NEW_AFTERSALERECORD = 251,
            AFTERSALE_APPLE = 252,
            PURCAHSE_DETAIL_VIEW = 500,
            UPDATETHIRDORDERNO = 601
        }

        /// <summary>
        /// 验证码场景
        /// </summary>
        public enum CaptchaType
        {
            REGISTER = 1,
            FINDPWD = 2,
            SAVESUCTOMER = 3,
            CHANGE_EMAIL = 4,
            CHANGEUSERINFO = 5,
        }

        /// <summary>
        /// 验证方式
        /// </summary>
        public enum VerifyWay
        {
            Phone = 1,
            Mail = 2
        }

        public enum CustomerType
        {
            [Description("企业")]
            COMPANY = 1,
            [Description("个人")]
            PERSON = 2
        }

        public enum CustomerLevel
        {
            BLACKIRON = 0
        }

        public enum Industry
        {
            [Description("旅行社")]
            TRAVLING = 1,
            [Description("休闲及餐饮工商业")]
            CATERING = 2,
            [Description("其它")]
            ELSE = 10
        }

        public enum CustomerAuditStatus
        {
            [Description("驳回")]
            REJECT = -5,
            [Description("待审核")]
            WAITAUDIT = 0,
            [Description("通过")]
            PASS = 10
        }

        public enum TranscactionType
        {
            [Description("下单")]
            BOOKING = 1,
            [Description("退款")]
            REFUND = 5
        }

        public enum TranscactionIncom
        {
            [Description("进账")]
            In = 1,
            [Description("出账")]
            Out = -1
        }

        public enum InvoiceOpeningMethod
        {
            AUTO = 1,
            MANUAL = 2
        }

        public enum InvoiceTileType
        {
            COMPANY = 1,
            PERSON = 2
        }

        //自己的发票类型
        public enum InvoiceType
        {
            VAT = 1
        }

        /// <summary>
        /// 开票方式(1携程开票，2酒店开票，3供应商开票)
        /// </summary>
        public enum CtripInvoiceType
        {
            [Description("携程开票")]
            CtripInvoice = 1,
            [Description("酒店开票")]
            HotelInvoice = 2,
            [Description("供应商开票")]
            SupplierInvoice = 3,
            [Description("其他")]
            OtherInvoice = 4,
        }

        public enum InvoiceFrom
        {
            PLATFORM = 1,
            HOTEL = 2,
            THIRD = 3
        }


        /// <summary>
        /// 业务类型  预付/担保
        /// </summary>
        public enum HotelBusinessType
        {
            [Description("酒店预付")]
            ADVANCE = 1,
            [Description("担保")]
            GUARANTEE = 2

        }


        public enum BlackType
        {
            [Description("临时")]
            TIMESPSN = 1,
            [Description("永久")]
            PERPETUAL = 2
        }

        public enum BlackStatus
        {
            [Description("过期")]
            OVERDUE = -1,
            [Description("生效")]
            EFFECT = 1,
            [Description("暂停")]
            STOP = 2
        }

        public enum BlackReason
        {
            [Description("酒店")]
            HOTEL = 1,
            [Description("平台")]
            PLATFORM = 3,
            [Description("价格")]
            PRICE = 5,
        }

        public enum SMSPlace
        {
            [Description("未知")]
            UNKNOWN = 0,
            [Description("注册")]
            REGISTER = 1,
            [Description("找回密码")]
            FINDPWD = 2,
            [Description("退款")]
            RRFUND = 5,
            [Description("完善信息")]
            SAVECUSTOMER = 6
        }

        public enum SMSKey
        {
            [Description("验证码")]
            CAPTCHA = 1
        }

        public enum SMSSendResult
        {
            [Description("失败")]
            FAIL = -1,
            [Description("未发送")]
            ORIGINAL = 0,
            [Description("成功")]
            SUCCESS = 1
        }

        public enum SMSProvider
        {
            [Description("腾讯云")]
            TENCENTCLOUD = 1,
            [Description("华为云")]
            HUAWEICLOUD = 5,
        }

        public enum SMSReportResult
        {
            [Description("未收到")]
            ORIGINAL = 0,
        }

        public enum Platform
        {
            //采购平台
            [Description("美团分销")]
            MEITUAN_FENXIAO = 6,
            [Description("携程商旅")]
            CTRIP_BUSINESS = 7,
            [Description("携程代订")]
            CTRIP_DAIDING = 8,
            [Description("铁路12306")]
            TIELU = 13,
            [Description("高德")]
            AUTONAVI = 3,
            [Description("智行")]
            ZHIXING = 11,

            //携程分销
            [Description("携程分销")]
            CTRIP_FENXIAO = 9,
            [Description("携程分销-众客")]
            CTRIP_ZHONGKE = 901,
            [Description("携程分销-众客促销")]
            CTRIP_ZHONGKEPROMITION = 902,
            [Description("携程分销-易住促销")]
            CTRIP_YIZHUPROMOTION = 903,
            [Description("携程分销-授信")]
            CTRIP_SHOUXIN = 904,
            [Description("携程分销-小猪促销")]
            CTRIP_XIAOZHUPROMITION = 905,

            //艺龙分销
            [Description("艺龙分销")]
            ELONG_FENXIAO = 4,
            [Description("艺龙M端")]
            ELONGTEPAI = 70,
            [Description("艺龙分销-易住")]
            ELONG_FENXIAOYIZHU = 401,
            [Description("艺龙分销-优优")]
            ELONG_FENXIAOYOUYOU = 402,
            [Description("艺龙优享会")]
            ELONG_YOUXIANGHUI = 403,
            //飞猪分销
            [Description("飞猪分销")]
            FEIZHU_FENXIAO = 17,
            [Description("锦江分销")]
            JINJIANG = 110,
            [Description("锦江分销-河南众客")]
            JINJIANG_FENXIAO_HENANZHONGKE = 1101,
            [Description("锦江分销-武汉承天广运")]
            JINJIANG_FENXIAO_WUHANCHENGTIAN = 1102,
            [Description("锦江分销-河南万山红建筑")]
            JINJIANG_FENXIAO_HENANWANSHANHONG = 1103,
            [Description("锦江分销-湖北鼎龙")]
            JINJIANG_FENXIAO_HUBEIDINGLONG = 1104,
            [Description("亚朵")]
            YADUO = 120,
            [Description("华住")]
            HUAZHU = 130,
            [Description("如家")]
            RUJIA = 140,
            [Description("百达星系")]
            BDXX = 150,
            [Description("百度地图")]
            BAIDU_MAP = 27,
            [Description("百度")]
            BAIDU = 160,
            //小猪
            [Description("快跑")]
            XIAOZHU = 12,
            [Description("快跑-易住账号")]
            KUAIPAO_YIZHU = 1201,
            [Description("快跑-YoYo账号")]
            KUAIPAO_YOYO = 1202,
            [Description("快跑-YoYo1开票账号")]
            KUAIPAO_YOYO1 = 1203,
            //直连商家
            [Description("51ZhuWeb")]
            WOYAOZHUWEB = 1,
            [Description("美团众客高星")]
            MEITUAN = 2,
            [Description("美团众客低星")]
            MEITUANZHONEKEDIXING = 16,
            [Description("美团优优高星")]
            MEITUANYOUYOUGAOXING = 29,

            [Description("飞猪荣春")]
            FEIZHU = 5,
            [Description("飞猪众客")]
            FEIZHUZHONGKE = 51,
            [Description("飞猪优优")]
            FEIZHUYOUYOU = 52,
            [Description("携程直连")]
            CTRIP = 10,
            [Description("携程同程住哲")]
            CTRIPZHUZHE = 28,
            [Description("武汉胜意")]
            WuHanShengYi = 15,
            [Description("捷旅直连")]
            Jielv = 18,
            [Description("铁旅")]
            TieLv = 19,
            [Description("深圳易沃思")]
            YIWOSI = 20,
            [Description("深圳涂涂")]
            TUTU = 21,
            [Description("广州蜗牛")]
            GUANGZHOUWONIU = 22,
            [Description("武汉胜意特牌")]
            WUHANSHENGYITEPAI = 23,
            [Description("兰溪指北针")]
            ZHIBEIZHEN = 24,
            [Description("去哪儿-众游旅行")]
            QUNAR_ZHONGYOU = 25,
            [Description("高德直连")]
            GaoDeDirection = 26,
            [Description("去哪儿-我要住")]
            QUNAR = 30,
            [Description("迈骐国际")]
            MAIQIGUOJI = 31,
            [Description("同程")]
            TONGCHENG = 40,
            [Description("京东")]
            JINGDONG = 50,
            [Description("艺龙App")]
            ELONG = 71,
            [Description("线下")]
            OFFLINE = 60,
            [Description("其它")]
            ELSE = 100,

        }
        /// <summary>
        /// 携程订单状态返回
        /// </summary>
        public enum CtripOrderStatus
        {
            [Description("未提交")]
            Uncommitted = 0,
            [Description("确认中")]
            Process = 2,
            [Description("已确认")]
            Confirm = 4,
            [Description("已取消")]
            Cancel = 6,
            [Description("成交")]
            Success = 8,
            [Description("满房")]
            Noroom = 10
        }

        public enum InvoiceProduceType
        {
            [Description("订单")]
            ORDER = 1,
            [Description("周期")]
            PERIOD = 5
        }

        public enum InvoiceStatus
        {
            [Description("已申请")]
            ORIGINAL = 0,
            [Description("开票中")]
            CONFIRM = 5,
            [Description("已开票")]
            DRAWER = 10,
        }

        public enum OrderInvoiceStatus
        {
            [Description("未申请")]
            ORIGINAL = 0,
            [Description("已申请")]
            REQUEST = 5,
            [Description("开票中")]
            CONFIRM = 10,
            [Description("已开票")]
            DRAWER = 20,
        }

        //（0-无窗；1-部分有窗；2-有窗；4-内窗；5-天窗；6-封闭窗；7-飘窗；unknow未知）
        public enum WindowStaus
        {
            [Description("无窗")]
            NoWindow = 0,
            [Description("部分有窗")]
            PartWindow = 1,
            [Description("有窗")]
            HasWindow = 2,
            [Description("内窗")]
            InWindow = 4,
            [Description("天窗")]
            SkyWindow = 5,
            [Description("封闭窗")]
            CloseWindow = 6,
            [Description("飘窗")]
            BayWindow = 7,
            [Description("未知")]
            Unknown = 8,
        }


        public enum InvoiceTargetType
        {
            [Description("携程开票")]
            CtripInvoice = 1,
            [Description("酒店开票")]
            HotelInvoice = 2,
            [Description("其他开票")]
            OtherInvoice = 3
        }

        /// <summary>
        /// 发票模式
        /// </summary>
        public enum InvoiceMode
        {

            [Description("普通开票")]
            HotelInvoice = 0,
            [Description("预约开票")]
            MakeMode = 1,
        }

        public enum BreakfastStatus
        {
            [Description("不含早")]
            NoBreakfast = 0,
            [Description("含单早")]
            SingleBreakfast = 1,
            [Description("含双早")]
            DoubleBreakfast = 2,
        }

        public enum TolietStatus
        {
            [Description("独立卫浴")]
            OwnToliet = 1,
            [Description("公共卫浴")]
            CommonToliet = 2,
            [Description("未知")]
            Unknown = -100,
        }


        public enum BroadnetStaus
        {
            [Description("无")]
            No = 0,
            [Description("全部房间有且收费")]
            AllHaveAndCharge = 1,
            [Description("全部房间有且免费")]
            AllHaveAndFree = 2,
            [Description("部分房间有且收费")]
            PartHaveAndCharge = 3,
            [Description("部分房间有且免费")]
            PartHaveAndFree = 4,
            [Description("未知")]
            UnKnown = -100,
        }

        /// <summary>
        /// 登录类型
        /// </summary>
        public enum LoginType
        {
            Login = 1,
            Logout = 2
        }

        public enum PurchaseMethod
        {
            [Description("系统")]
            SYSTEM = 1,
            [Description("人工")]
            MANNAL = 2
        }

        /// <summary>
        /// 操作酒店/房型上下线方式
        /// </summary>
        public enum OnLineMethod
        {
            [Description("系统")]
            SYSTEM = 1,
            [Description("人工")]
            MANNAL = 2
        }
        /// <summary>
        /// 采购单状态
        /// </summary>
        public enum PurchaseOrderStatus
        {
            //[Description("采购中/可定检查")]
            //PurchaseOrderCheck = 5,
            //[Description("采购中/订单保存")]
            //PurchaseOrderSave = 10,
            //[Description("采购中/订单提交")]
            //PurchaseOrderSubmit = 15,
            [Description("已取消")]
            Cancel = -10,
            [Description("待采购")]
            WaitPurchase = 10,
            [Description("采购中")]
            Purchasing = 15,
            [Description("采购失败")]
            PurchaseFail = 20,
            [Description("采购成功")]
            PurchaseSuccess = 25,
            [Description("待确认")]
            WaitConfirm = 30,
            [Description("确认中")]
            Confirming = 35,
            [Description("确认失败")]
            ConfirmFail = 40,
            [Description("已确认")]
            ConfirmSuccess = 45,
            [Description("已退款")]
            Refunds = 50,
            [Description("待付款")]
            WaitPay = 55
        }

        /// <summary>
        ///设施类型 1-酒店设施 2-房型设施
        /// </summary>
        public enum FacilityType
        {
            [Description("酒店设施")]
            HotelFacility = 1,
            [Description("房型设施")]
            RoomTypeFacility = 2
        }

        /// <summary>
        ///酒店/房型更新 1-酒店 2-房型
        /// </summary>
        public enum HotelMatchType
        {
            [Description("酒店")]
            Hotel = 1,
            [Description("房型")]
            RoomType = 2
        }

        /// <summary>
        /// 消息类型
        /// </summary>
        public enum MessageTypeEnum
        {
            info = 0,
            success = 1,
            warning = -5,
            error = -100
        }

        public enum MessageRedirectType
        {
            order = 1,
        }

        public enum InOnline
        {
            [Description("不在线")]
            OFFLINE = 0,
            [Description("在线")]
            ONLINE = 1
        }

        public enum NoticePublishStatus
        {
            [Description("未发布")]
            NO = 0,
            [Description("已发布")]
            YES = 1
        }

        public enum NoticeIsTopStatus
        {
            [Description("未置顶")]
            NO = 0,
            [Description("置顶")]
            YES = 1
        }

        /// <summary>
        /// 校验房型库存时返回Code
        /// </summary>
        public enum RoomCheckCode
        {
            [Description("校验成功")]
            Success = 0,
            [Description("校验失败")]
            Fail = 1,
            [Description("黑名单酒店")]
            BlackList = 2,
            [Description("房间不可定")]
            NotBook = 3,
            [Description("三方产品不可售")]
            NotSale = 4,
            [Description("产品不存在")]
            RoomNotFound = 5,
            [Description("库存不足")]
            Stockout = 6,
            [Description("未获取到价格")]
            NotFoundPrice = 7,
            [Description("价格发生变化")]
            PriceChange = 8,
            [Description("无效Token")]
            InvalidToken = 404,
            [Description("接口请求频次超限")]
            Limit = 405,
            [Description("价格上涨")]
            PriceRise = 206
        }

        /// <summary>
        /// 订单类型
        /// </summary>
        public enum OrderType
        {
            [Description("人工订单列表")]
            ManualOrder = 1001,
            [Description("人工待处理订单列表")]
            ManualUnHandleOrder = 1002,
            [Description("系统订单列表")]
            SysOrder = 2001,
            [Description("系统待处理订单列表")]
            SysUnHandleOrder = 2002,
            [Description("全部订单列表")]
            AllOrder = 3000,
        }

        /// <summary>
        /// 采购账号类型
        /// </summary>
        public enum PurChaseAccountType
        {
            [Description("新账号")]
            NEW_ACCOUNT = 1,
            [Description("老账号")]
            OLD_ACCOUNT = 5,
            [Description("超级会员")]
            SUPER_VIP = 10,
            [Description("银卡会员")]
            SLIVE_VIP = 15,
            [Description("黄金会员")]
            GOLD_VIP = 20,
            [Description("白金会员")]
            PLATINUM_VIP = 25,
            [Description("其它")]
            ELSE = 100,
        }

        /// <summary>
        /// 人工采购-暂存/提交
        /// </summary>
        public enum PurChaseStaging
        {
            [Description("暂存")]
            Stating = 0,
            [Description("提交")]
            Submit = 1
        }

        public enum AfterSaleApplyStatus
        {
            [Description("待处理")]
            ORIGINAL = 0,
            [Description("处理中")]
            HANDLING = 5,
            [Description("处理完成")]
            COMPLATE = 10,
            [Description("已关闭")]
            CLOSE = -1
        }

        public enum AfterSaleType
        {
            [Description("其它")]
            ELSE = 1,
            [Description("退款")]
            REFUND = 5
        }

        public enum AfterSaleReason
        {
            [Description("到店无房")]
            ARRIVE_NOROOM = 5,
            [Description("查无预订")]
            NONE_BOOKING = 10,
            [Description("酒店不接待")]
            NOT_WELCOME = 15,
            [Description("酒店设施")]
            HOTEL_DEIVCE = 20,
            [Description("客户因素")]
            CUSTOMER = 25,
            [Description("不可抗因素")]
            NOCAN = 30,
            [Description("其它")]
            ELSE = 100,
        }

        public enum AfterSaleCallType
        {
            [Description("呼出")]
            IN = 2,
            [Description("呼入")]
            OUT = 1
        }

        public enum ModuleType
        {
            [Description("页面")]
            PAGE = 10,
            [Description("按钮/接口")]
            API = 20
        }

        public enum ResourceType
        {
            [Description("预订")]
            BOOKING = 2,
            [Description("催单")]
            Reminder = 3,
            [Description("支付")]
            PAY = 4,
            [Description("取消支付")]
            CANCAEL_PAY = 5,
            [Description("中止采购")]
            ORDER_SUSPENG_PURCHASE = 6,
            [Description("重新采购")]
            ORDER_REPURCHASE = 8,
            [Description("转人工处理")]
            ORDER_TURNTO_MANUAL = 10,
            [Description("锁单")]
            LOCK_ORDER = 12,
            [Description("解锁订单")]
            UNLOCK_ORDER = 14,
            [Description("预订成功")]
            ORDER_BOOKING_SUCCESS = 16,
            [Description("预订失败")]
            ORDER_BOOKING_FAIL = 18,
            [Description("拒单")]
            ORDER_REJECT = 20,
            [Description("取消订单")]
            CANCEL_ORDER = 24,
            [Description("新建采购单")]
            NRE_PURCHASE_ORDER = 102,
            [Description("修改采购单")]
            EDIT_PURCHASE_ORDER = 103,
            [Description("采购成功")]
            PURCUASE_SUCCESS = 110,
            [Description("采购失败")]
            PURCUASE_FAIL = 111,
            [Description("采购退款")]
            PURCUASE_REFUND = 112,
            [Description("申请退款")]
            REFUND_APPLY = 201,
            [Description("退款")]
            REFUND_TRADE = 202,
            [Description("退款通过")]
            REFUND_CONFIRM = 203,
            [Description("退款驳回")]
            REFUND_REJECT = 204,
            [Description("申请售后")]
            AFTERSALE_APPLY = 302,
            [Description("售后处理锁定")]
            AFTERSALE_HANDLE_LOCK = 303,
            [Description("售后处理解锁")]
            AFTERSALE_HANDLE_UNLOCK = 304,
            [Description("售后处理完成")]
            AFTERSALE_HANDLE_COMPLATE = 305,
            [Description("新增售后记录")]
            NEW_AFTERSALE_RECORD = 310,
            [Description("修改售后记录")]
            EDIT_AFTERSALE_RECORD = 311,
            [Description("售后记录通话结束")]
            AFTERSALE_RECORD_CALLEND = 311,
            [Description("新增后台用户")]
            NEW_SYSUSER = 501,
            [Description("新增子用户")]
            NEW_SYSSUBUSER = 5011,
            [Description("修改后台用户")]
            EDIT_SYSUSER = 502,
            [Description("重置后台用户密码")]
            SYSUSER_RESETPWD = 503,
            [Description("删除用户")]
            DELETE_SYSUSER = 504,
            [Description("用户禁用")]
            SYSUSER_DISABLED = 505,
            [Description("用户启用")]
            SYSUSER_ENABLED = 506,
            [Description("用户角色修改")]
            SYSUSER_ROLE_EDIT = 507,
            [Description("用户用户组修改")]
            SYSUSER_USERGROUP_EDIT = 508,
            [Description("用户组用户修改")]
            USERGROUP_USER__EDIT = 509,
            [Description("用户组模块修改")]
            USERGROUP_MODEULE__EDIT = 510,
            [Description("找回密码")]
            FIND_PWD = 550,
            [Description("客户审核通过")]
            CUSTOMER_AUDIT_PASS = 551,
            [Description("客户审核驳回")]
            CUSTOMER_AUDIT_REJECT = 552,
            [Description("注册")]
            REGISTER = 580,
            [Description("修改密码")]
            CHANGE_PWD = 582,
            [Description("新增公告")]
            NEW_NOTICE = 601,
            [Description("修改公告")]
            EDIT_NOTICE = 602,
            [Description("置顶公告")]
            TOP_NOTICE = 603,
            [Description("取消置顶公告")]
            UNTOP_NOTICE = 604,
            [Description("发布公告")]
            PUBLISH_NOTICE = 605,
            [Description("撤回公告")]
            UNPUBLISH_NOTICE = 606,
            [Description("新增菜单")]
            NEW_MENU = 701,
            [Description("修改菜单")]
            EDIT_MENU = 702,
            [Description("删除菜单")]
            DELETE_MENU = 703,
            [Description("启用菜单")]
            ENDABLED_MENU = 704,
            [Description("禁用菜单")]
            DISABLED_MENU = 705,
            [Description("新增角色")]
            NEW_ROLE = 750,
            [Description("修改角色")]
            EDIT_ROLE = 751,
            [Description("删除角色")]
            DELETE_ROLE = 752,
            [Description("修改角色菜单")]
            EDIT_ROLE_MENU = 753,
            [Description("新增模块")]
            NEW_MODULE = 801,
            [Description("修改模块")]
            EDIT_MODULE = 802,
            [Description("删除模块")]
            DELETE_MODULE = 803,
            [Description("新增用户组")]
            NEW_USERGROUP = 851,
            [Description("修改用户组")]
            EDIT_USERGROUP = 852,
            [Description("删除用户组")]
            DELETE_USERGROUP = 853,
            [Description("新增系统配置")]
            NEW_SYSCONFIG = 901,

            [Description("修改系统配置")]
            EDIT_SYSCONFIG = 902,

            [Description("删除系统配置")]
            DELETE_SYSCONFIG = 905,
            [Description("新增酒店报价配置")]
            NEW_HOTELOFFERCONFIG = 908,
            [Description("修改酒店报价配置")]
            EDIT_HOTELOFFERCONFIG = 909,
            [Description("删除酒店报价配置")]
            DELETE_HOTELOFFERCONFIG = 910,


            [Description("新增订单分流配置")]
            NEW_ORDERSHUNTCONFIG = 915,
            [Description("修改订单分流配置")]
            EDIT_ORDERSHUNTCONFIG = 916,
            [Description("删除订单分流配置")]
            DELETE_ORDERSHUNTCONFIG = 917,

            [Description("删除短信模板")]
            DELETE_SMSTEMPLATE = 920,
            [Description("新增短信模板")]
            NEW_SMSTEMPLATE = 921,
            [Description("修改短信模板")]
            EDIT_SMSTEMPLATE = 922,
            [Description("新增短信平台")]
            NEW_SMSPLATFORM = 925,
            [Description("修改短信平台")]
            EDIT_SMSPLATFORM = 926,
            [Description("新增/修改系统限流配置")]
            Config_Throttle = 951,
            [Description("采购详情查看")]
            PURCAHSE_DETAIL_VIEW = 1000,
            [Description("客户身份证图片查看")]
            CUSTOMER_IDCARD_VIEW = 1100,
            [Description("客户营业执照图片查看")]
            CUSTOMER_BUSINES_VIEW = 1101,
            [Description("全部订单导出")]
            EXPORT_ALLORDER = 2001,
            [Description("系统订单导出")]
            EXPORT_SYSTEMORDER = 2002,
            [Description("系统待办订单导出")]
            EXPORT_WAITSYSTEMORDER = 2003,
            [Description("人工订单导出")]
            EXPORT_MUANALORDER = 2004,
            [Description("人工待办订单导出")]
            EXPORT_WAITMUANALORDER = 2005,
            [Description("退款列表导出")]
            EXPORT_REFUNDORDER = 2010,
            [Description("账户交易列表导出")]
            EXPORT_ACCOUNTTRAN = 2011,
            [Description("采购单列表导出")]
            EXPORT_PURCHASEORDER = 2012,
            [Description("客户订单导出")]
            CUSTOMER_EXPORT_ALLORDER = 2101,
            [Description("客户退款列表导出")]
            CUSTOMER_EXPORT_REFUNDORDER = 2102,
            [Description("客户账户交易列表导出")]
            CUSTOMER_EXPORT_ACCOUNTTRAN = 2103,
            [Description("创建钱包账户")]
            CreateAccount = 3001,
            [Description("钱包账户充值")]
            AccountRecharge = 3002,
            [Description("酒店上下架")]
            PLATFORMHOTELUPLOW = 4001,
            [Description("酒店拉黑")]
            PLATFORMHOTELBLOCK = 4002,
        }

        public enum RegisterHandlerStatus
        {
            EXCEPTION = -10,
            FAIL = -1,
            ORIGAL = 0,
            SUCCESS = 1
        }

        /// <summary>
        /// 接口房间是否可取消
        /// </summary>
        public enum CancelType
        {
            [Description("免费取消")]
            FreeCancel = 1,
            [Description("限时取消")]
            TimeCancel = 2,
            [Description("不可取消")]
            NoCancel = 3
        }

        public enum PushStatus
        {
            [Description("推送成功")]
            SUCCESS = 100,
            [Description("未推送")]
            ORIGAL = 0,
            [Description("正在推送")]
            PUSHING = 10,
            [Description("推送失败")]
            FAIL = -1,
            [Description("推送异常")]
            EXCEPTION = -10
        }

        /// <summary>
        /// 单据类型
        /// </summary>
        public enum BillType
        {
            ORDER = 1
        }

        /// <summary>
        /// 授权类型
        /// </summary>
        public enum AuthorizationType
        {
            SIGN = 1,
            JWT = 2
        }

        /// <summary>
        /// 携程账号ID
        /// </summary>
        public enum CtripAccountId
        {
            ZHONGKE = 1,
            ZHONGKEPROMITION = 2,
            XIAOZHUPROMITION = 10,
        }

        public enum FeiZhuStatusChangeType
        {
            HOTEL = 1,
            ROOMTYPE = 2
        }



        /// <summary>
        /// 账户交易类型
        /// </summary>
        public enum AccountTransactionType
        {
            [Description("充值")]
            RECHARGE = 1,
            [Description("提现")]
            CASHOUT = 2,
            [Description("消费")]
            EXPENSE = 3,
            [Description("退款")]
            REFUND = 4,
        }


        /// <summary>
        /// 账户操作类型
        /// </summary>
        public enum AccountOperationType
        {
            [Description("线上")]
            ONLINE = 1,
            [Description("线下")]
            OFFLINE = 2,
            [Description("调整")]
            ADJUSTMENT = 3,
        }

        /// <summary>
        /// 账户金额交易类型
        /// </summary>
        public enum AccountIncomeType
        {
            [Description("入账")]
            INCOME = 1,
            [Description("支出")]
            EXPEND = -1
        }

        /// <summary>
        /// 账户状态
        /// </summary>
        public enum AccountStatus
        {
            [Description("正常")]
            NORMAL = 1,
            [Description("冻结")]
            FREEZE = 2
        }

        public enum MeiTuanResponseCode
        {
            请求成功 = 0,
            无数据 = 1,
            系统异常请稍后重试 = 2,
            美团认证失败 = 12,
            参数值无效 = 18,
            价格校验失败 = 1001,
            库存校验失败 = 1002,
            价格库存都校验失败 = 1003,
            关房 = 1006
        }

        public enum MeiTuanOrderValidate
        {
            VALIDATE_SUCCESS = 0,
            VALIDATE_FAIL = -1
        }

        public enum MeiTuanOrderBook
        {
            [Description("预定成功")]
            BOOK_SUCCESS = 1,
            [Description("预定失败")]
            BOOK_FAIL = 5,
            [Description("预定中")]
            BOOKING = 10,
            [Description("取消成功")]
            CANCEL_SUCCESS = 15
        }

        public enum MeiTuanOrderCancel
        {
            [Description("取消成功")]
            CANCEL_SUCCESS = 1,
            [Description("取消失败")]
            CANCEL_FAIL = 5,
        }

        /// <summary>
        /// 第三方(携程/小猪)的订单状态
        /// </summary>
        public enum ThirdOrderStatus
        {
            [Description("订单已创建")]
            Init = 1001,
            [Description("未提交")]
            Uncommitted = 1002,
            [Description("确认中")]
            Process = 1003,
            [Description("订单待确认")]
            WaitConfirm = 1004,
            [Description("订单已拒绝")]
            Reject = 1005,
            [Description("订单超时")]
            Overtime = 1006,
            [Description("订单下单失败")]
            Failed = 1007,
            [Description("订单已确认")]
            Confirmed = 1008,
            [Description("订单已取消")]
            Canceled = 1009,
            [Description("订单已成交")]
            Success = 1010,
            [Description("订单已满房")]
            Noroom = 1012,
            [Description("异常情况")]
            Exception = 1013,
        }

        /// <summary>
        /// 酒店报价配置溢价设置
        /// </summary>
        public enum HotelPremiumType
        {
            [Description("百分比")]
            PREMIUMRATE = 1,
            [Description("金额")]
            PREMIUMAMOUNT = 2,
            [Description("百分比,金额")]
            BOTH = 3

        }

        /// <summary>
        /// 订单风险等级
        /// </summary>
        public enum OrderRiskLevel
        {
            [Description("无风险")]
            NoRisk = 1,
            [Description("低风险")]
            LowRisk = 2,
            [Description("中低风险")]
            LowAndMiddleRisk = 3,
            [Description("中风险")]
            Middle = 4,
            [Description("未知风险")]
            UnKnownRisk = 5,
            [Description("中高风险")]
            MiddleAndHighRisk = 6,
            [Description("高风险")]
            HighRisk = 7
        }


        //订单人工操作：待拒单 1、已拒单 2
        public enum RejectionStatus
        {
            [Description("待拒单")]
            WaitReject = 1,
            [Description("已拒单")]
            Rejected = 2
        }

    }
}
