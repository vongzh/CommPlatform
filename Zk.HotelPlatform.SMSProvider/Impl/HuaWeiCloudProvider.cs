using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Zk.HotelPlatform.Model;
using Zk.HotelPlatform.Model.Third.HuaWeiCloud;
using Zk.HotelPlatform.Utils;
using Zk.HotelPlatform.Utils.Global;

namespace Zk.HotelPlatform.SMSProvider.Impl
{
    public class HuaWeiCloudProvider : ISMSProvider
    {
        public void SendSMS(SMS sms)
        {
            try
            {
                //为防止因HTTPS证书认证失败造成API调用失败,需要先忽略证书信任问题
                HttpClient client = new HttpClient();
                ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

                //请求Headers
                client.DefaultRequestHeaders.Add("Authorization", "WSSE realm=\"SDP\",profile=\"UsernameToken\",type=\"Appkey\"");
                client.DefaultRequestHeaders.Add("X-WSSE", BuildWSSEHeader(GlobalConfig.HuaWeiSMS_AppKey, GlobalConfig.HuaWeiSMS_AppSecret));

                //请求Body
                var param = JsonConvert.DeserializeObject<Dictionary<string, string>>(sms.Param);
                var body = new Dictionary<string, string>() {
                    {"from", GlobalConfig.HuaWeiSMS_Channel},
                    {"to", sms.Mobile},
                    {"templateId", sms.PlatTemplateId},
                    {"templateParas", JsonConvert.SerializeObject(param.Values.ToArray())},
                    {"statusCallback", GlobalConfig.HuaWeiSMS_ReportAPI},
                    {"signature", GlobalConfig.SMS_Signtrue}, //使用国内短信通用模板时,必须填写签名名称
                    {"extend",sms.Id.ToString() }
                };

                HttpContent content = new FormUrlEncodedContent(body);

                var response = client.PostAsync(GlobalConfig.HuaWeiSMS_API, content).Result;
                var res = response.Content.ReadAsStringAsync().Result;
                if (string.IsNullOrWhiteSpace(res))
                    throw new BusinessException("短信接口未收到返回");
                var result = JsonConvert.DeserializeObject<BaseResponse<List<SendSMSResponse>>>(res);
                if (result.code != "0000")
                {
                    sms.SendCode = result.code;
                    sms.SendMsg = result.description;
                    sms.SendRet = (int)GlobalEnum.SMSSendResult.FAIL;
                }
                else
                {
                    var sendRet = result.result[0];
                    sms.RequestId = sendRet.smsMsgId;
                    sms.SendRet = sendRet.status == "0000" ? (int)GlobalEnum.SMSSendResult.SUCCESS : (int)GlobalEnum.SMSSendResult.FAIL;
                }
            }
            catch (Exception ex)
            {
                sms.SendMsg = ex.Message;
            }
        }

        /// <summary>
        /// 构造X-WSSE参数值
        /// </summary>
        /// <param name="appKey"></param>
        /// <param name="appSecret"></param>
        /// <returns></returns>
        private static string BuildWSSEHeader(string appKey, string appSecret)
        {
            string now = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ssZ"); //Created
            string nonce = Guid.NewGuid().ToString().Replace("-", ""); //Nonce

            byte[] material = Encoding.UTF8.GetBytes(nonce + now + appSecret);
            byte[] hashed = SHA256Managed.Create().ComputeHash(material);
            string hexdigest = BitConverter.ToString(hashed).Replace("-", "");
            string base64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(hexdigest)); //PasswordDigest

            return String.Format("UsernameToken Username=\"{0}\",PasswordDigest=\"{1}\",Nonce=\"{2}\",Created=\"{3}\"",
                            appKey, base64, nonce, now);
        }
    }
}
