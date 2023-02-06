using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zk.HotelPlatform.CacheModel;

namespace Zk.HotelPlatform.CacheProvider
{
    public interface ICaptchaCacheProvider
    {
        bool SetCaptcha(string type, string dataKey, CaptchaUse val);
        //bool SetCaptchaCount_Mobile(string type, string dataKey, int sendCount);
        //int GetCaptchaCount_Mobile(string type, string dataKey);
        //bool SetCaptchaCount_IP(string type, IpSend ipSend);
        //List<IpSend> GetCaptchaCount_IP(string type);
        CaptchaUse GetCaptcha(string type, string dataKey);
        //bool RemoveCaptcha(string type, string dataKey);
        List<string> GetSensitiveWord();
        //  bool SetMailCaptcha(string type, string dataKey, CaptchaUse val);
        //  CaptchaUse GetMailCaptcha(string type, string dataKey);
        // bool RemoveMailCaptcha(string type, string dataKey);
    }
}
