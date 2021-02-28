namespace WebsiteBanVeXe.Models
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class BanVeXeDbContext : DbContext
    {
        public BanVeXeDbContext()
            : base("name=ChuoiKn")
        {
        }
        public virtual DbSet<user> Users { get; set; }
        public virtual DbSet<menu> menus { get; set; }
        public virtual DbSet<Coach> coach { get; set; }
        public virtual DbSet<order> orders { get; set; }
        public virtual DbSet<ordersdetail> ordersdetails { get; set; }

        public virtual DbSet<ticket> tickets { get; set; }
        public virtual DbSet<post> Posts { get; set; }
        public virtual DbSet<role> roles { get; set; }
        public virtual DbSet<topic> Topics { get; set; }

      
    }
}
