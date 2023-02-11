using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zk.HotelPlatform.Model.Request
{
    public class CourseQueryRequest
    {
        public string CourseName { get; set; }

        public int? IsDelete { get; set; }
    }
}
