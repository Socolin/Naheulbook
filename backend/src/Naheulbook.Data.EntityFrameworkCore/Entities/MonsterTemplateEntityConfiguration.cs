using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Naheulbook.Data.EntityFrameworkCore.Entities;

public class MonsterTemplateEntityConfiguration : IEntityTypeConfiguration<MonsterTemplateEntity>
{
    public void Configure(EntityTypeBuilder<MonsterTemplateEntity> builder)
    {
        builder.ToTable("monster_templates");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .HasColumnName("id");
        builder.Property(e => e.Data)
            .HasColumnName("data")
            .HasColumnType("json");
        builder.Property(e => e.Name)
            .HasColumnName("name");
        builder.Property(e => e.SubCategoryId)
            .HasColumnName("subCategoryId");

        builder.HasOne(x => x.SubCategory)
            .WithMany(x => x.MonsterTemplates)
            .HasForeignKey(x => x.SubCategoryId)
            .HasConstraintName("FK_monster_template_monster_category_categoryid");
    }
}