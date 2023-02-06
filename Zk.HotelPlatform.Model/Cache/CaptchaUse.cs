using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zk.HotelPlatform.CacheModel
{
    public class CaptchaUse
    {
        public string RequestId { get; set; }
        public string Code { get; set; }
        public DateTime SendTime { get; set; }
        public int SendCount { get; set; }
        public int Valid { get; set; }
        public DateTime ExpireTime { get; set; }
    }

    public class IpSend
    {
        public string Ip { get; set; }
        public int SendCount { get; set; }
    }


}
