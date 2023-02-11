using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zk.HotelPlatform.Model.Request
{
    public class CreateOrderRequest
    {
        public int CourseId { get; set; }
        public int SchemeId { get; set; }

        public DateTime BeginClassTime { get; set; }
        public Signup Signup { get; set; }
    }
}
