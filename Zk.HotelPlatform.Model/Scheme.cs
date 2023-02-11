using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zk.HotelPlatform.Model
{
    public class Scheme : BaseEntity
    {
        public string SchemeName { get; set; }
        public decimal ServiceRate { get; set; }
        public int TotalNumber { get; set; }
    }
}
