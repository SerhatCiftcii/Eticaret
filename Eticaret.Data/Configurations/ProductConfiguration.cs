using Eticaret.Core.Entities; //apuserdan alındı 
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eticaret.Data.Configurations
{
    internal class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.Property(x => x.Name).HasMaxLength(150);
            builder.Property(x => x.Image).HasMaxLength(100);
            builder.Property(x => x.ProductCode).HasMaxLength(50);
            builder.Property(x => x.Price)
    .HasColumnType("decimal(18,2)") // Burada 18, toplam basamak sayısı ve 2, ondalık basamağı belirtir
    .IsRequired();


        }
    }
}
