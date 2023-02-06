using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zk.HotelPlatform.Model
{
    [Table("Captcha")]
    public class Captcha
    {
        [Key]
        public int Id { get; set; }
        public string Mobile { get; set; }
        public string IP { get; set; }
        public DateTime Time { get; set; }
        public DateTime? EndTime { get; set; }
        public int CaptchaCode { get; set; }
        public int EvilLevel { get; set; }
        public string RequestId { get; set; }
        public string EndRequestId { get; set; }
        public int CaptchaType { get; set; }
        public int VerifyWay { get; set; }
        public int IsUse { get; set; }
    }
}
