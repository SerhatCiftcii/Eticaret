using Eticaret.Core.Entities; //apuserdan alındı 
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eticaret.Data.Configurations
{
    internal class BrandConfiguration : IEntityTypeConfiguration<Brand>
    {
 //fluent api  aslında  string ve zorunlu alanlar için düzenleme yapıyoruz.Descrtption uzun olacağı için ayar yapılmadı. 
public void Configure(EntityTypeBuilder<Brand> builder)
        {
        builder.Property(x => x.Name).IsRequired().HasMaxLength(50);//.HasColumnType("varchar(50)") bunu hep vermek zorundadeğiliz migratıonda kendisi ayarlıyabilir.
        builder.Property(x => x.Logo).HasMaxLength(50);




        }
    }
}
