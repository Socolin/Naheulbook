using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Naheulbook.Data.Models;

namespace Naheulbook.Data.Configurations
{
    public class ItemTemplateCategoryConfiguration : IEntityTypeConfiguration<ItemTemplateCategory>
    {
        public void Configure(EntityTypeBuilder<ItemTemplateCategory> builder)
        {
            builder.ToTable("item_template_category");

            builder.HasIndex(e => e.SectionId)
                .HasName("IX_item_category_type");

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
                .HasMaxLength(255);;

            builder.Property(e => e.SectionId)
                .IsRequired()
                .HasColumnName("section");

            builder.HasOne(e => e.Section)
                .WithMany(s => s.Categories)
                .HasForeignKey(e => e.SectionId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_item_category_item_type_type");
        }
    }

    public class ItemTemplateSectionConfiguration : IEntityTypeConfiguration<ItemTemplateSection>
    {
        public void Configure(EntityTypeBuilder<ItemTemplateSection> builder)
        {
            builder.ToTable("item_template_section");

            builder.Property(e => e.Id)
                .HasColumnName("id");

            builder.Property(e => e.Name)
                .IsRequired()
                .HasColumnName("name")
                .HasMaxLength(255);

            builder.Property(e => e.Note)
                .IsRequired()
                .HasColumnName("note");

            builder.Property(e => e.Special)
                .IsRequired()
                .HasColumnName("special")
                .HasMaxLength(2048);
        }
    }
}