using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Naheulbook.Data.EntityFrameworkCore.Entities;

public class ItemTemplateSubCategoryEntityConfiguration : IEntityTypeConfiguration<ItemTemplateSubCategoryEntity>
{
    public void Configure(EntityTypeBuilder<ItemTemplateSubCategoryEntity> builder)
    {
        builder.ToTable("item_template_subcategories");

        builder.HasIndex(e => e.SectionId)
            .HasDatabaseName("IX_item_category_type");

        builder.Property(e => e.Id)
            .HasColumnName("id");

        builder.Property(e => e.Description)
            .IsRequired()
            .HasColumnName("description")
            .HasMaxLength(255);

        builder.Property(e => e.Name)
            .IsRequired()
            .HasColumnName("name")
            .HasMaxLength(255);

        builder.Property(e => e.Note)
            .IsRequired()
            .HasColumnName("note");

        builder.Property(e => e.TechName)
            .IsRequired()
            .HasColumnName("techname")
            .HasDefaultValueSql("''")
            .HasMaxLength(255);

        builder.Property(e => e.SectionId)
            .IsRequired()
            .HasColumnName("section");

        builder.HasOne(e => e.Section)
            .WithMany(s => s.SubCategories)
            .HasForeignKey(e => e.SectionId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_item_category_item_type_type");
    }
}