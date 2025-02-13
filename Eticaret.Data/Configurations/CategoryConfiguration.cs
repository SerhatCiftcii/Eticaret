using Eticaret.Core.Entities; //apuserdan alındı 
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eticaret.Data.Configurations
{
    internal class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.Property(x => x.Name).IsRequired().HasMaxLength(50);//.HasColumnType("varchar(50)") bunu hep vermek zorundadeğiliz migratıonda kendisi ayarlıyabilir.
            builder.Property(x => x.Image).HasMaxLength(50);
            builder.HasData(
                new Category
                {
                    Id = 1,
                    Name = "Elektronik",
                    IsActive = true,
                    IsTopMenu = true,
                    ParentId = 0,
                    OrderNo = 1,
                },
                 new Category
                 {
                     Id = 2,
                     Name = "BiLgisayar",
                     IsActive = true,
                     IsTopMenu = true,
                     ParentId = 0,
                     OrderNo = 2,
                 });



        }
    }
}
