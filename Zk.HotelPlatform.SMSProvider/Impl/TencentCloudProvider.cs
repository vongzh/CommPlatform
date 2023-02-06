using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TencentCloud.Common;
using TencentCloud.Sms.V20190711;
using TencentCloud.Sms.V20190711.Models;
using Zk.HotelPlatform.Model;
using Zk.HotelPlatform.Utils.Global;

namespace Zk.HotelPlatform.SMSProvider.Impl
{
    public class TencentCloudProvider : ISMSProvider
    {
        public void SendSMS(SMS sms)
        {
            try
            {
                var client = new SmsClient(new Credential()
                {
                    SecretId = GlobalConfig.TencentCloud_SecretId,
                    SecretKey = GlobalConfig.TencentCloud_SecretKey

                }, string.Empty);

                var param = JsonConvert.DeserializeObject<Dictionary<string, string>>(sms.Param);
                var sendSmsResponse = client.SendSms(new SendSmsRequest()
                {
                    Sign = GlobalConfig.SMS_Signtrue,
                    TemplateID = sms.PlatTemplateId,
                    TemplateParamSet = param.Values.ToArray(),
                    SmsSdkAppid = GlobalConfig.QCloudSMS_AppId,
                    PhoneNumberSet = new string[1] { sms.Mobile },

                }).Result;
                sms.RequestId = sendSmsResponse.RequestId;

                var sendRet = sendSmsResponse.SendStatusSet[0];
                if (sendRet.Code == "OK")
                {
                    sms.SendRet = (int)GlobalEnum.SMSSendResult.SUCCESS;
                    sms.SendCode = sendRet.Code;
                }
                else
                {
                    sms.SendRet = (int)GlobalEnum.SMSSendResult.FAIL;
                    sms.SendMsg = sendRet.Message;
                    sms.SendCode = sendRet.Code;
                }
            }
            catch (Exception ex)
            {
                sms.SendMsg = ex.Message;
            }
        }
    }
}
