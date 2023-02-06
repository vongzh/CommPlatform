using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zk.HotelPlatform.Model;

namespace Zk.HotelPlatform.SMSProvider
{
    public interface ISMSProvider
    {
        void SendSMS(SMS sms);
    }
}
