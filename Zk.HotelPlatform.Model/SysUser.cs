namespace Zk.HotelPlatform.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Sys_User")]
    public class SysUser
    {
        [Key]
        public int Id { get; set; }
        public int IsDelete { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime? UpdateTime { get; set; }
        public int UserId { get; set; }
        [Required]
        [StringLength(20)]
        public string UserName { get; set; }

        [StringLength(20)]
        public string NickName { get; set; }

        [Required]
        [StringLength(32)]
        public string Pwd { get; set; }

        [StringLength(50)]
        public string Mail { get; set; }

        [StringLength(11)]
        public string Mobile { get; set; }

        [Required]
        [StringLength(32)]
        public string Salt { get; set; }

        [StringLength(200)]
        public string Avatar { get; set; }

        public int Status { get; set; }
        public int Disabled { get; set; }
        public DateTime? LastLoginTime { get; set; }
        public int? LoginCount { get; set; }
        public int? LoginErrorCount { get; set; }
        public DateTime? LoginErrorTime { get; set; }
        public int UserType { get; set; }
        [StringLength(20)]
        public string CustomerNo { get; set; }
        public string DepartmentId { get; set; }

        public int? ParentUserId { get; set; }
        public string QyWorkUserId { get; set; }
        public int? UserPlatformId { get; set; }
    }
}
