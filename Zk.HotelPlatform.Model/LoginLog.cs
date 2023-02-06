namespace Zk.HotelPlatform.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    

    [Table("LoginLog")]
    public partial class LoginLog
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        [StringLength(100)]
        public string LoginIP { get; set; }

        [StringLength(50)]
        public string UserName { get; set; }

        public DateTime? LoginTime { get; set; }

        [StringLength(500)]
        public string LoginDevice { get; set; }

        public int? LoginType { get; set; }
        public string LoginRemark { get; set; }
    }
}
