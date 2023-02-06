using Newtonsoft.Json;
using Senparc.CO2NET.Helpers;
using Senparc.Weixin;
using Senparc.Weixin.Helpers;
using Senparc.Weixin.TenPay;
using Senparc.Weixin.TenPay.V3;
using System;
using System.Collections;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using Zk.HotelPlatform.Utils;
using Zk.HotelPlatform.Utils.Global;
using Zk.HotelPlatform.Utils.Log;

namespace Zk.HotelPlatform.Service.Impl
{
    public class TencentPayService : IPayService
    {
        private TenPayV3Info _tenPayV3Info = null;

        public TencentPayService()
        {
            if (_tenPayV3Info == null)
            {
                var key = TenPayHelper.GetRegisterKey(Config.SenparcWeixinSetting);

                _tenPayV3Info = TenPayV3InfoCollection.Data[key];
            }
        }

        public (bool RefundResult, string Message) RefundTrade(string paymentNo, string refundNo, decimal paymentAmount, decimal refundAmount)
        {
            var notifyUrl = GlobalConfig.TenPayCallBackUrl;
            var dataInfo = new TenPayV3RefundRequestData(_tenPayV3Info.AppId, _tenPayV3Info.MchId, _tenPayV3Info.Key,
                null, TenPayV3Util.GetNoncestr(), null, paymentNo, refundNo, (int)(paymentAmount * 100), (int)(refundAmount * 100), _tenPayV3Info.MchId, null, notifyUrl: notifyUrl);

            var cert = GlobalConfig.TenPay_Cert;
            var result = TenPayV3.Refund(null, dataInfo, cert, _tenPayV3Info.MchId);
            LogInfoWriter.GetInstance().Info(JsonConvert.SerializeObject(result));

            if (result.result_code == "SUCCESS")
            {
                return (true, string.Empty);
            }

            return (false, result.err_code_des);
        }

        public (bool PayResult, string Message) CloseTrade(string paymentOrderNo)
        {
            return (false, "未实现");
        }

        /*public (bool Result, string Message, TradeCreateResponse Data) CreateTrade(TradeCreateRequest tradeCreateRequest)
        {
            try
            {
                var requestData = new TenPayV3UnifiedorderRequestData(_tenPayV3Info.AppId,
            _tenPayV3Info.MchId,
            tradeCreateRequest.ProductName + tradeCreateRequest.ProductDesc,
            tradeCreateRequest.PaymentOrderNo,
            (int)(tradeCreateRequest.Amount * 100),
            tradeCreateRequest.ClientIP,
            _tenPayV3Info.TenPayV3Notify,
           TenPayV3Type.NATIVE,
            null,
            _tenPayV3Info.Key,
            TenPayV3Util.GetNoncestr(),
            timeStart: tradeCreateRequest.Date,
            timeExpire: tradeCreateRequest.Date.AddMinutes(30),
            productId: tradeCreateRequest.OrderNo);
                //调用统一订单接口
                var result = TenPayV3.Unifiedorder(requestData);
                LogInfoWriter.GetInstance().Info(JsonConvert.SerializeObject(result));
                if (result.result_code == "SUCCESS")
                {
                    var prepayNo = result.prepay_id;
                    var qrcode = QRCodeUtil.Encode(result.code_url);
                    return (true, null, new TradeCreateResponse() { PrePayNo = prepayNo, Body = qrcode });
                }
                return (false, result.return_msg, null);
            }
            catch (Exception ex)
            {
                LogInfoWriter.GetInstance().Error(ex);
                return (false, null, null);
            }
        }*/

        public (bool PayResult, string Message, string tradrNo) QueryTradeResult(string paymentOrderNo, decimal orderAmount)
        {
            throw new NotImplementedException();
        }
    }

    public class TenpayResponseHandle
    {
        /// <summary>
        /// 密钥 
        /// </summary>
        private string Key;

        /// <summary>
        /// appkey
        /// </summary>
        private string Appkey;

        /// <summary>
        /// 应答的参数
        /// </summary>
        protected Hashtable Parameters;

        /// <summary>
        /// debug信息
        /// </summary>
        private string DebugInfo;
        /// <summary>
        /// 原始内容
        /// </summary>
        protected string Content;

        private string Charset = "gb2312";

        /// <summary>
        /// 初始化函数
        /// </summary>
        public virtual void Init()
        {

        }

        /// <summary>
        /// 获取页面提交的get和post参数
        /// 注意:.NetCore环境必须传入HttpContext实例，不能传Null，这个接口调试特别困难，千万别出错！
        /// </summary>
        /// <param name="httpContext"></param>
        public TenpayResponseHandle(string param)
        {
            Parameters = new Hashtable();

            XmlDocument xmlDoc = new Senparc.CO2NET.ExtensionEntities.XmlDocument_XxeFixed();
            xmlDoc.XmlResolver = null;
            xmlDoc.LoadXml(param);
            XmlNode root = xmlDoc.SelectSingleNode("xml");
            XmlNodeList xnl = root.ChildNodes;

            foreach (XmlNode xnf in xnl)
            {
                this.SetParameter(xnf.Name, xnf.InnerText);
            }
        }

        /// <summary>
        /// 获取密钥
        /// </summary>
        /// <returns></returns>
        public string GetKey()
        { return Key; }

        /// <summary>
        /// 设置密钥
        /// </summary>
        /// <param name="key"></param>
        public void SetKey(string key)
        {
            this.Key = key;
        }

        /// <summary>
        /// 获取参数值
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public string GetParameter(string parameter)
        {
            string s = (string)Parameters[parameter];
            return (null == s) ? "" : s;
        }

        /// <summary>
        /// 设置参数值
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="parameterValue"></param>
        public void SetParameter(string parameter, string parameterValue)
        {
            if (parameter != null && parameter != "")
            {
                if (Parameters.Contains(parameter))
                {
                    Parameters.Remove(parameter);
                }

                Parameters.Add(parameter, parameterValue);
            }
        }

        /// <summary>
        /// 是否财付通签名,规则是:按参数名称a-z排序,遇到空值的参数不参加签名。return boolean
        /// </summary>
        /// <returns></returns>
        public virtual Boolean IsTenpaySign()
        {
            StringBuilder sb = new StringBuilder();

            ArrayList akeys = new ArrayList(Parameters.Keys);
            akeys.Sort(ASCIISort.Create());

            foreach (string k in akeys)
            {
                string v = (string)Parameters[k];
                if (null != v && "".CompareTo(v) != 0
                    && "sign".CompareTo(k) != 0 && "key".CompareTo(k) != 0)
                {
                    sb.Append(k + "=" + v + "&");
                }
            }

            sb.Append("key=" + this.GetKey());
            string sign = EncryptHelper.GetMD5(sb.ToString(), GetCharset()).ToLower();
            this.SetDebugInfo(sb.ToString() + " &sign=" + sign);
            //debug信息
            return GetParameter("sign").ToLower().Equals(sign);
        }

        /// <summary>
        /// 获取debug信息
        /// </summary>
        /// <returns></returns>
        public string GetDebugInfo()
        { return DebugInfo; }

        /// <summary>
        /// 设置debug信息
        /// </summary>
        /// <param name="debugInfo"></param>
        protected void SetDebugInfo(String debugInfo)
        { this.DebugInfo = debugInfo; }

        protected virtual string GetCharset()
        {
#if NET45
            return this.HttpContext.Request.ContentEncoding.BodyName;
#else
            return Encoding.UTF8.WebName;
#endif
        }

        /// <summary>
        /// 输出XML
        /// </summary>
        /// <returns></returns>
        public string ParseXML()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<xml>");
            foreach (string k in Parameters.Keys)
            {
                string v = (string)Parameters[k];
                if (Regex.IsMatch(v, @"^[0-9.]$"))
                {

                    sb.Append("<" + k + ">" + v + "</" + k + ">");
                }
                else
                {
                    sb.Append("<" + k + "><![CDATA[" + v + "]]></" + k + ">");
                }

            }
            sb.Append("</xml>");
            return sb.ToString();
        }
    }
}
