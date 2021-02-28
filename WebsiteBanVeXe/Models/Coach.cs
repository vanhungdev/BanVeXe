namespace WebsiteBanVeXe.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Coach")]
    public partial class Coach
    {
        [Key]
        public int id { get; set; }
        public string name { get; set; }

        public string description { get; set; }

        public string address { get; set; }
        public string licensePlate { get; set; }

        public int seatTotal { get; set; }
        public string img { get; set; }
        public int status { get; set; }
    }
}
