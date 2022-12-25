using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Naheulbook.Data.Models;

namespace Naheulbook.Data.Configurations
{
    public class ItemTemplateConfiguration : IEntityTypeConfiguration<ItemTemplateEntity>
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

    public class ItemTemplateSubCategoryConfiguration : IEntityTypeConfiguration<ItemTemplateSubCategoryEntity>
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

    public class ItemTemplateSectionConfiguration : IEntityTypeConfiguration<ItemTemplateSectionEntity>
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

    public class ItemTemplateModifierConfiguration : IEntityTypeConfiguration<ItemTemplateModifierEntity>
    {
        public void Configure(EntityTypeBuilder<ItemTemplateModifierEntity> builder)
        {
            builder.ToTable("item_template_modifiers");

            builder.HasIndex(e => e.ItemTemplateId)
                .HasDatabaseName("IX_item_template_modifiers_itemTemplateId");

            builder.HasIndex(e => e.RequiredJobId)
                .HasDatabaseName("IX_item_template_modifiers_requiredJobId");

            builder.HasIndex(e => e.RequiredOriginId)
                .HasDatabaseName("IX_item_template_modifiers_requiredOriginId");

            builder.HasIndex(e => e.StatName)
                .HasDatabaseName("IX_item_effect_stat");

            builder.Property(e => e.Id)
                .HasColumnName("id");

            builder.Property(e => e.ItemTemplateId)
                .IsRequired()
                .HasColumnName("itemTemplateId");

            builder.Property(e => e.RequiredJobId)
                .HasColumnName("requiredJobId");

            builder.Property(e => e.RequiredOriginId)
                .HasColumnName("requiredOriginId");

            builder.Property(e => e.Special)
                .IsRequired(false)
                .HasColumnName("special")
                .HasMaxLength(2048);

            builder.Property(e => e.StatName)
                .IsRequired()
                .HasColumnName("stat")
                .HasMaxLength(64);

            builder.Property(e => e.Type)
                .IsRequired()
                .HasColumnName("type")
                .HasMaxLength(64)
                .HasDefaultValueSql("'ADD'");

            builder.Property(e => e.Value)
                .HasColumnName("value");

            builder.HasOne(s => s.ItemTemplate)
                .WithMany(e => e.Modifiers)
                .HasForeignKey(s => s.ItemTemplateId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_item_template_modifiers_itemTemplateId_item_templates_id");

            builder.HasOne(s => s.RequiredOrigin)
                .WithMany()
                .HasForeignKey(s => s.RequiredOriginId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_item_template_modifiers_requiredOriginId_origins_id");

            builder.HasOne(s => s.RequiredJob)
                .WithMany()
                .HasForeignKey(s => s.RequiredJobId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_item_template_modifiers_requiredJobId_jobs_id");
        }
    }

    public class ItemTemplateRequirementConfiguration : IEntityTypeConfiguration<ItemTemplateRequirementEntity>
    {
        public void Configure(EntityTypeBuilder<ItemTemplateRequirementEntity> builder)
        {
            builder.ToTable("item_template_requirements");

            builder.HasIndex(e => e.ItemTemplateId);

            builder.HasIndex(e => e.StatName);

            builder.Property(e => e.Id)
                .HasColumnName("id");

            builder.Property(e => e.ItemTemplateId)
                .HasColumnName("itemTemplateId");

            builder.Property(e => e.MaxValue)
                .HasColumnName("maxvalue");

            builder.Property(e => e.MinValue)
                .HasColumnName("minvalue");

            builder.Property(e => e.StatName)
                .IsRequired()
                .HasColumnName("stat")
                .HasMaxLength(64);

            builder.HasOne(e => e.ItemTemplate)
                .WithMany(i => i.Requirements)
                .HasForeignKey(r => r.ItemTemplateId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_item_template_requirements_itemTemplateId_item_templates_id");

            builder.HasOne(e => e.Stat)
                .WithMany()
                .HasForeignKey(r => r.StatName)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_item_requirement_stat_stat");
        }
    }

    public class ItemTemplateSkillConfiguration : IEntityTypeConfiguration<ItemTemplateSkillEntity>
    {
        public void Configure(EntityTypeBuilder<ItemTemplateSkillEntity> builder)
        {
            builder.ToTable("item_template_skills");

            builder.HasKey(e => new {e.SkillId, e.ItemTemplateId});

            builder.HasIndex(e => e.ItemTemplateId)
                .HasDatabaseName("IX_item_template_skills_itemTemplateId");

            builder.HasIndex(e => e.SkillId)
                .HasDatabaseName("IX_item_template_skills_skillId");

            builder.Property(e => e.ItemTemplateId)
                .HasColumnName("itemTemplateId");

            builder.Property(e => e.SkillId)
                .HasColumnName("skillId");

            builder.HasOne(e => e.Skill)
                .WithMany()
                .HasForeignKey(e => e.SkillId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_item_template_skills_skillId_skills_id");

            builder.HasOne(e => e.ItemTemplate)
                .WithMany(i => i.Skills)
                .HasForeignKey(s => s.ItemTemplateId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_item_template_skills_itemTemplateId_item_templates_id");
        }
    }

    public class ItemTemplateSkillModifiersConfiguration : IEntityTypeConfiguration<ItemTemplateSkillModifierEntity>
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

    public class ItemTemplateSlotConfiguration : IEntityTypeConfiguration<ItemTemplateSlotEntity>
    {
        public void Configure(EntityTypeBuilder<ItemTemplateSlotEntity> builder)
        {
            builder.HasKey(e => new {Slot = e.SlotId, Item = e.ItemTemplateId});

            builder.ToTable("item_template_slots");

            builder.HasIndex(e => e.ItemTemplateId)
                .HasDatabaseName("IX_item_template_slots_itemTemplateId");

            builder.Property(e => e.SlotId)
                .HasColumnName("slot");

            builder.Property(e => e.ItemTemplateId)
                .HasColumnName("itemTemplateId");

            builder.HasOne(s => s.ItemTemplate)
                .WithMany(s => s.Slots)
                .HasForeignKey(s => s.ItemTemplateId)
                .HasConstraintName("FK_item_template_slots_itemTemplateId_item_templates_id");

            builder.HasOne(s => s.Slot)
                .WithMany()
                .HasForeignKey(s => s.SlotId)
                .HasConstraintName("FK_item_template_slot_item_slot_slot");
        }
    }

    public class ItemTemplateUnSkillConfiguration : IEntityTypeConfiguration<ItemTemplateUnSkillEntity>
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

    public class ItemTypeConfiguration : IEntityTypeConfiguration<ItemTypeEntity>
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

    public class SlotConfiguration : IEntityTypeConfiguration<SlotEntity>
    {
        public void Configure(EntityTypeBuilder<SlotEntity> builder)
        {
            builder.ToTable("slots");

            builder.Property(e => e.Id)
                .HasColumnName("id");

            builder.Property(e => e.Count)
                .HasColumnName("count");

            builder.Property(e => e.Name)
                .IsRequired()
                .HasColumnName("name")
                .HasMaxLength(255);

            builder.Property(e => e.Stackable)
                .HasColumnName("stackable");

            builder.Property(e => e.TechName)
                .IsRequired()
                .HasColumnName("techname")
                .HasMaxLength(255);
        }
    }
}