using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Naheulbook.Data.EntityFrameworkCore.Entities;

public class ItemTemplateUnSkillEntityConfiguration : IEntityTypeConfiguration<ItemTemplateUnSkillEntity>
{
    public void Configure(EntityTypeBuilder<ItemTemplateUnSkillEntity> builder)
    {
        builder.ToTable("item_template_unskills");

        builder.HasKey(e => new {e.SkillId, e.ItemTemplateId});

        builder.HasIndex(e => e.ItemTemplateId)
            .HasDatabaseName("IX_item_template_unskills_itemTemplateId");

        builder.HasIndex(e => e.SkillId)
            .HasDatabaseName("IX_item_template_unskills_skillId");

        builder.Property(e => e.ItemTemplateId)
            .HasColumnName("itemTemplateId");

        builder.Property(e => e.SkillId)
            .HasColumnName("skillId");

        builder.HasOne(e => e.Skill)
            .WithMany()
            .HasForeignKey(e => e.SkillId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_item_template_unskills_skillId_skills_id");

        builder.HasOne(e => e.ItemTemplate)
            .WithMany(i => i.UnSkills)
            .HasForeignKey(s => s.ItemTemplateId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_item_template_unskills_itemTemplateId_item_templates_id");
    }
}