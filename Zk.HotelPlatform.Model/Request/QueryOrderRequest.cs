using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using Zk.HotelPlatform.Model.Basic.Pager;

namespace Zk.HotelPlatform.Model.Request
{
    public class QueryOrderRequest: PageRequest
    {
        public string OrderNo { get; set; }
        public string UserName { get; set; }
        public string CourseName { get; set; }
        public string SchemeName { get; set; }
        public int OrderStatus { get; set; }
        public DateTime? CreateTimeBegin { get; set; }
        public DateTime? CreateTimeEnd { get; set; }
    }
}
