using Eticaret.Core.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Eticaret.Data
{
    public class DatabaseContext : DbContext
    {
        public DbSet<Address> Addresses { get; set; }
        public DbSet<AppUser> AppUsers { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<News> News { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Slider> Sliders { get; set; }

        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderLine> OrderLines { get; set; }
       

        //yapılandırmaları (configureleri tanıtıcaz efcora) 
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=SERHAT\SQLEXPRESS; 
            Database=EcommerceDb; Trusted_Connection=True; TrustServerCertificate=True;");
            base.OnConfiguring(optionsBuilder);
        }
        //yapılandırmaları (configureleri tanıtıcaz efcora) 
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
           // modelBuilder.ApplyConfiguration(new Configurations.AppUserConfiguration());
           // modelBuilder.ApplyConfiguration(new Configurations.BrandConfiguration());
           
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());// çalışan dll in classları kendi otomtoik ayarlar üstteki gibi tek tek tanıtmaya gerek kalmaz

             // Cascade Delete ayarı: AppUser silinince Address'ler de silinsin yapılmazsa adresi olan kullnacılar silinirken hata fırlatır.
            modelBuilder.Entity<Address>()
                .HasOne(a => a.AppUser)
                .WithMany(u => u.Addresses)
                .HasForeignKey(a => a.AppUserId)
                .OnDelete(DeleteBehavior.Cascade);

            base.OnModelCreating(modelBuilder);
        }



    }
}
