using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zk.HotelPlatform.Model;
using Zk.HotelPlatform.Utils;
using Zk.HotelPlatform.Utils.Global;

namespace Zk.HotelPlatform.Service
{
    public interface ICaptchaService
    {
        (bool Result, string RequestId) SlideVerification(string mobile, string randstr, string ticket, int ret, string ip, GlobalEnum.CaptchaType captchType);
        bool SendCaptcha(string mobile, string code, string requestId, GlobalEnum.CaptchaType type, string ip);
        (bool Result, string RequestId) VerifyCaptcha(string mobile, string code, GlobalEnum.CaptchaType captchType);
        bool IsUse(string mobile, string requestId);
        Captcha GetCaptcha(string mobile, string requestId);
        bool IsUse(Captcha captcha);
        bool VerifyMailCaptcha(string mail, string code, GlobalEnum.CaptchaType captchType);
        bool SensitiveWordFilter(string word);
        bool SendPayCodeMail(string mailUrl, string code);
    }
}
