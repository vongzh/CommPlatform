using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zk.HotelPlatform.Model
{
    public class Course : BaseEntity
    {
        public string CourseName { get; set; }
        public int Price { get; set; }
        public int Duration { get; set; }
    }
}
