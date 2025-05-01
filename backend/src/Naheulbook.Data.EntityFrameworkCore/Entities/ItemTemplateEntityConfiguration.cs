using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Naheulbook.Data.EntityFrameworkCore.Entities;

public class ItemTemplateEntityConfiguration : IEntityTypeConfiguration<ItemTemplateEntity>
{
    public void Configure(EntityTypeBuilder<ItemTemplateEntity> builder)
    {
        builder.ToTable("item_templates");

        builder.HasIndex(e => e.SubCategoryId);

        builder.Property(e => e.Id)
            .HasColumnName("id");

        builder.Property(e => e.SubCategoryId)
            .HasColumnName("subCategoryId");

        builder.Property(e => e.CleanName)
            .HasColumnName("cleanname")
            .HasMaxLength(255)
            .HasDefaultValueSql("NULL");

        builder.Property(e => e.Data)
            .HasColumnName("data")
            .HasColumnType("json");

        builder.Property(e => e.Name)
            .IsRequired()
            .HasColumnName("name")
            .HasMaxLength(255);

        builder.Property(e => e.TechName)
            .HasColumnName("techname")
            .HasMaxLength(255);

        builder.Property(e => e.SourceUserNameCache)
            .HasColumnName("sourceusernamecache")
            .HasColumnType("text");
        builder.Property(e => e.Source)
            .HasColumnName("source")
            .HasColumnType("text");

        builder.Property(e => e.SourceUserId)
            .HasColumnName("sourceuserid");

        builder.HasOne(e => e.SubCategory)
            .WithMany(c => c.ItemTemplates)
            .HasForeignKey(e => e.SubCategoryId)
            .HasConstraintName("FK_item_template_item_category_category");

        builder.HasOne(e => e.SourceUser)
            .WithMany()
            .IsRequired(false)
            .HasForeignKey(e => e.SourceUserId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}

public class ItemTemplateSkillModifiersEntityConfiguration : IEntityTypeConfiguration<ItemTemplateSkillModifierEntity>
{
    public void Configure(EntityTypeBuilder<ItemTemplateSkillModifierEntity> builder)
    {
        builder.ToTable("item_template_skill_modifiers");

        builder.HasIndex(e => e.ItemTemplateId)
            .HasDatabaseName("IX_item_template_skill_modifiers_itemTemplateId");

        builder.HasIndex(e => e.SkillId)
            .HasDatabaseName("IX_item_template_skill_modifiers_skillId");

        builder.Property(e => e.Id)
            .HasColumnName("id");

        builder.Property(e => e.ItemTemplateId)
            .HasColumnName("itemTemplateId");

        builder.Property(e => e.SkillId)
            .HasColumnName("skillId");

        builder.Property(e => e.Value)
            .HasColumnName("value");

        builder.HasOne(e => e.Skill)
            .WithMany()
            .HasForeignKey(e => e.SkillId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_item_template_skill_modifiers_itemTemplateId");

        builder.HasOne(e => e.ItemTemplate)
            .WithMany(i => i.SkillModifiers)
            .HasForeignKey(e => e.ItemTemplateId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_item_template_skill_modifiers_skillId_skills_id");
    }
}