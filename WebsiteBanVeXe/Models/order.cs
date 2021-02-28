namespace WebsiteBanVeXe.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("order")]
    public partial class order
    {
        [Key]
        public int ID { get; set; }
        public int guestTotal { get; set; }
        public string code { get; set; }
        public string name { get; set; }
        public string gioitinh { get; set; }
        public string quoctich { get; set; }
        public string mess { get; set; }
        public string phone { get; set; }
        public string email { get; set; }
        public string deliveryPaymentMethod { get; set; }
        public int StatusPayment { get; set; }
        public double total { get; set; }
        public DateTime created_ate { get; set; }
        public int status { get; set; }
    }
}
