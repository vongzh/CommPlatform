using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zk.HotelPlatform.Model
{
    [Table("PaymentOrder")]
    public partial class PaymentOrder
    {
        public int Id { get; set; }

        public int? UserId { get; set; }

        [StringLength(100)]
        public string OrderNo { get; set; }

        [StringLength(100)]
        public string PaymentNo { get; set; }

        public int? TotalAmount { get; set; }
        public int? ClassAmount { get; set; }
        public int? ServiceAmount { get; set; }

        public DateTime? PaymentTime { get; set; }

        public int PaymentStatus { get; set; }

        [StringLength(100)]
        public string PaymentSerialNo { get; set; }

        [StringLength(200)]
        public string Remark { get; set; }

        public int IsDelete { get; set; }

        public DateTime CreateTime { get; set; }

        public DateTime? ModifyTime { get; set; }
        public string OpenId { get; set; }
        public int Periods { get; set; }
    }
}
