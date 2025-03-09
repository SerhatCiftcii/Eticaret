using Eticaret.Core.Entities; //apuserdan alındı 
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Eticaret.Data.Configurations
{
    internal class AppUserConfiguration : IEntityTypeConfiguration<AppUser>
    {
        public void Configure(EntityTypeBuilder<AppUser> builder)
        {
            //proprty x AppUSERA DENK GELİR COLONTPİPDE ColumnType("varchar(50)") İLE VERİ TABANINDA Kİ KARŞILIĞI BELİRLENİR hasMaxLenght(50) İLEDE MAX UZUNLUK BELİRLENİR
            builder.Property(x => x.Name).IsRequired().HasColumnType("varchar(50)").HasMaxLength(50);
            builder.Property(x => x.Surname).IsRequired().HasColumnType("varchar(50)").HasMaxLength(50);
            builder.Property(x => x.Email).IsRequired().HasColumnType("varchar(50)").HasMaxLength(50);
            builder.Property(x => x.Phone).HasColumnType("varchar(15)").HasMaxLength(15);
            builder.Property(x => x.Password).IsRequired().HasColumnType("nvarchar(50)").HasMaxLength(50);
            builder.Property(x => x.UserName).HasColumnType("varchar(50)").HasMaxLength(50);

            builder.Property(x => x.VerificationCode)
           .HasColumnType("varchar(6)")  // Doğrulama kodu genellikle 6 karakter uzunluğunda olur
           .HasMaxLength(6)
           .IsRequired(false);  // Opsiyonel olduğundan Required false yapılabilir.

            //birde seed data lazım veritabanı oluştuktansonra bir kullanıcı kaydına yanı admın kaydına ihtiyaç var ki bu bilgilerle içine girsin
            builder.HasData(
                new AppUser
                {
                    Id = 1,
                    CreateDate = DateTime.Now,
                    UserName = "Admin",
                    Email = "admin@serhat.com",
                    IsActive = true,
                    IsAdmin = true,
                    Name = "Test",
                    Password="12345Aa",
                    Surname = "Test"
                });
    } }
}
