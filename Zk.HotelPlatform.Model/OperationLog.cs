namespace Zk.HotelPlatform.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    

    [Table("OperationLog")]
    public partial class OperationLog
    {
        [Key]
        public int Id { get; set; }
        public string LogId { get; set; }
        [StringLength(20)]
        public string UserId { get; set; }
        [StringLength(50)]
        public string UserName { get; set; }
        public int? UserType { get; set; }
        public string LogContent { get; set; }
        public string ChangedFields { get; set; }
        public int? ResourceType { get; set; }
        public string BusinessParam { get; set; }
        public string Result { get; set; }
        public DateTime OptTime { get; set; }
    }
}
