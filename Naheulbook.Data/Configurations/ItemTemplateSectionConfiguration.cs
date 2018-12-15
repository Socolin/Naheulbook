using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Naheulbook.Data.Models;

namespace Naheulbook.Data.Configurations
{
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