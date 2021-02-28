namespace WebsiteBanVeXe.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("topic")]
    public partial class topic
    {
        [Key]
        public int ID { get; set; }
        public string name { get; set; }
        public string slug { get; set; }
        public int parentid { get; set; }
        public int orders { get; set; }
        public DateTime created_at { get; set; }
        public int created_by { get; set; }
        public DateTime updated_at { get; set; }
        public int updated_by { get; set; }
        public int status { get; set; }
    }
}
