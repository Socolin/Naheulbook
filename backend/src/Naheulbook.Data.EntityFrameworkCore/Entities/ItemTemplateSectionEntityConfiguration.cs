using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Naheulbook.Data.EntityFrameworkCore.Entities;

public class ItemTemplateSectionEntityConfiguration : IEntityTypeConfiguration<ItemTemplateSectionEntity>
{
    public void Configure(EntityTypeBuilder<ItemTemplateSectionEntity> builder)
    {
        builder.ToTable("item_template_sections");

        builder.Property(e => e.Id)
            .HasColumnName("id");

        builder.Property(e => e.Name)
            .IsRequired()
            .HasColumnName("name")
            .HasMaxLength(255);

        builder.Property(e => e.Note)
            .IsRequired()
            .HasColumnName("note");

        builder.Property(e => e.Icon)
            .IsRequired()
            .HasMaxLength(64)
            .HasColumnName("icon");

        builder.Property(e => e.Special)
            .IsRequired()
            .HasColumnName("special")
            .HasMaxLength(2048);
    }
}