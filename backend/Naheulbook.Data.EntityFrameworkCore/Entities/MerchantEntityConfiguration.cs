using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Naheulbook.Data.EntityFrameworkCore.Entities;

public class MerchantEntityConfiguration : IEntityTypeConfiguration<MerchantEntity>
{
    public void Configure(EntityTypeBuilder<MerchantEntity> builder)
    {
        builder.ToTable("merchants");

        builder.Property(e => e.Id)
            .HasColumnName("id");

        builder.Property(e => e.Name)
            .IsRequired(true)
            .HasColumnName("name")
            .HasMaxLength(255);
    }
}