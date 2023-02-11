namespace Zk.HotelPlatform.Model
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Signup")]
    public class Signup : BaseEntity
    {
        [Required]
        [StringLength(10)]
        public string Name { get; set; }

        [StringLength(50)]
        public string QQ { get; set; }

        [StringLength(50)]
        public string Wechat { get; set; }

        [StringLength(50)]
        public string Email { get; set; }

        [StringLength(50)]
        public string Educational { get; set; }

        [StringLength(50)]
        public string UrgentContactPersonName { get; set; }

        [StringLength(50)]
        public string UrgentContactPersonPhone { get; set; }

        [StringLength(50)]
        public string UrgentContactPersonRelation { get; set; }

        [StringLength(50)]
        public string UrgentContactPersonAddress { get; set; }
    }
}
