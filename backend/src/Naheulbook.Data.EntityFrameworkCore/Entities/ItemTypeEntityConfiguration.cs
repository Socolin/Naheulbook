using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Naheulbook.Data.EntityFrameworkCore.Entities;

public class ItemTypeEntityConfiguration : IEntityTypeConfiguration<ItemTypeEntity>
{
    public void Configure(EntityTypeBuilder<ItemTypeEntity> builder)
    {
        builder.ToTable("item_types");

        builder.Property(e => e.Id)
            .HasColumnName("id");

        builder.Property(e => e.DisplayName)
            .IsRequired()
            .HasColumnName("displayName")
            .HasMaxLength(255);

        builder.Property(e => e.TechName)
            .IsRequired()
            .HasColumnName("techname")
            .HasMaxLength(255);
    }
}