namespace WebsiteBanVeXe.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ticket")]
    public partial class ticket
    {
        [Key]
        public int id { get; set; }
        public string code { get; set; }
        public int? garageId { get; set; }
        public string name { get; set; }
        public string airport { get; set; }
        public string description { get; set; }
        public string CoachName { get; set; }
        public string img { get; set; }
        public string departure_address { get; set; }
        public string arrival_address { get; set; }
        public DateTime departure_date { get; set; }
        public DateTime day { get; set; }
        public int guestTotal { get; set; }
        public string licensePlates { get; set; }
        public double price { get; set; }
        public int sold { get; set; }
        public DateTime created_at { get; set; }
        public int created_by { get; set; }
        public DateTime updated_at { get; set; }
        public int updated_by { get; set; }
        public int status { get; set; }
    }
}
