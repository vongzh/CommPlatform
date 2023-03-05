namespace Zk.HotelPlatform.Model
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("OrderInfo")]
    public class OrderInfo : BaseEntity
    {
        public string OrderNo { get; set; }
        public int? CourseId { get; set; }

        [StringLength(50)]
        public string CourseName { get; set; }

        public int? Duration { get; set; }

        public int? Price { get; set; }

        public int? SchemeId { get; set; }

        [StringLength(50)]
        public string SchemeName { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? ServiceRate { get; set; }

        public int? SchemeNum { get; set; }

        public int? Status { get; set; }

        public int? TotalAmount { get; set; }

        public int? UserId { get; set; }

        [StringLength(50)]
        public string UserName { get; set; }

        public int? SchemePayNum { get; set; }

        public int? PaymentAmount { get; set; }

        public DateTime BeginClassTime { get; set; }
        public int? PaymentServiceAmount { get; set; }
        public int TotalServiceAmount { get; set; }
        public string OpenId { get; set; }
    }
}
