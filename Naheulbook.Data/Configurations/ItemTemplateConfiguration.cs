using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Naheulbook.Data.Models;

namespace Naheulbook.Data.Configurations
{
    public class ItemTemplateConfiguration : IEntityTypeConfiguration<ItemTemplate>
    {
        public void Configure(EntityTypeBuilder<ItemTemplate> builder)
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

    public class ItemTemplateSubCategoryConfiguration : IEntityTypeConfiguration<ItemTemplateSubCategory>
    {
        public void Configure(EntityTypeBuilder<ItemTemplateSubCategory> builder)
        {
            builder.ToTable("item_template_subcategories");

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

    public class ItemTemplateSectionConfiguration : IEntityTypeConfiguration<ItemTemplateSection>
    {
        public void Configure(EntityTypeBuilder<ItemTemplateSection> builder)
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

            builder.Property(e => e.Special)
                .IsRequired()
                .HasColumnName("special")
                .HasMaxLength(2048);
        }
    }

    public class ItemTemplateModifierConfiguration : IEntityTypeConfiguration<ItemTemplateModifier>
    {
        public void Configure(EntityTypeBuilder<ItemTemplateModifier> builder)
        {
            builder.ToTable("item_template_modifiers");

            builder.HasIndex(e => e.ItemTemplateId)
                .HasName("IX_item_effect_item");

            builder.HasIndex(e => e.RequireJobId)
                .HasName("IX_item_effect_requirejob");

            builder.HasIndex(e => e.RequireOriginId)
                .HasName("IX_item_effect_requireorigin");

            builder.HasIndex(e => e.StatName)
                .HasName("IX_item_effect_stat");

            builder.Property(e => e.Id)
                .HasColumnName("id");

            builder.Property(e => e.ItemTemplateId)
                .IsRequired()
                .HasColumnName("itemTemplate");

            builder.Property(e => e.RequireJobId)
                .HasColumnName("requirejob");

            builder.Property(e => e.RequireOriginId)
                .HasColumnName("requireorigin");

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
                .HasConstraintName("FK_item_effect_item_template_item");
        }
    }

    public class ItemTemplateRequirementConfiguration : IEntityTypeConfiguration<ItemTemplateRequirement>
    {
        public void Configure(EntityTypeBuilder<ItemTemplateRequirement> builder)
        {
            builder.ToTable("item_template_requirements");

            builder.HasIndex(e => e.ItemTemplateId);

            builder.HasIndex(e => e.StatName);

            builder.Property(e => e.Id)
                .HasColumnName("id");

            builder.Property(e => e.ItemTemplateId)
                .HasColumnName("itemTemplate");

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
                .HasConstraintName("FK_item_requirement_item_template_item");

            builder.HasOne(e => e.Stat)
                .WithMany()
                .HasForeignKey(r => r.StatName)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_item_requirement_stat_stat");
        }
    }

    public class ItemTemplateSkillConfiguration : IEntityTypeConfiguration<ItemTemplateSkill>
    {
        public void Configure(EntityTypeBuilder<ItemTemplateSkill> builder)
        {
            builder.ToTable("item_template_skills");

            builder.HasKey(e => new {e.SkillId, e.ItemTemplateId});

            builder.HasIndex(e => e.ItemTemplateId)
                .HasName("IX_item_skill_item");

            builder.HasIndex(e => e.SkillId)
                .HasName("IX_item_template_skills_skillId");

            builder.Property(e => e.ItemTemplateId)
                .HasColumnName("itemTemplate");

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
                .HasConstraintName("FK_item_skill_item_template_item");
        }
    }

    public class ItemTemplateSkillModifiersConfiguration : IEntityTypeConfiguration<ItemTemplateSkillModifier>
    {
        public void Configure(EntityTypeBuilder<ItemTemplateSkillModifier> builder)
        {
            builder.ToTable("item_template_skill_modifiers");

            builder.HasIndex(e => e.ItemTemplateId)
                .HasName("IX_item_skill_modifiers_item");

            builder.HasIndex(e => e.SkillId)
                .HasName("IX_item_template_skill_modifiers_skillId");

            builder.Property(e => e.Id)
                .HasColumnName("id");

            builder.Property(e => e.ItemTemplateId)
                .HasColumnName("itemTemplate");

            builder.Property(e => e.SkillId)
                .HasColumnName("skillId");

            builder.Property(e => e.Value)
                .HasColumnName("value");

            builder.HasOne(e => e.Skill)
                .WithMany()
                .HasForeignKey(e => e.SkillId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_item_skill_modifiers_skill_skillId");

            builder.HasOne(e => e.ItemTemplate)
                .WithMany(i => i.SkillModifiers)
                .HasForeignKey(e => e.ItemTemplateId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_item_template_skill_modifiers_skillId_skills_id");
        }
    }

    public class ItemTemplateSlotConfiguration : IEntityTypeConfiguration<ItemTemplateSlot>
    {
        public void Configure(EntityTypeBuilder<ItemTemplateSlot> builder)
        {
            builder.HasKey(e => new {Slot = e.SlotId, Item = e.ItemTemplateId});

            builder.ToTable("item_template_slots");

            builder.HasIndex(e => e.ItemTemplateId)
                .HasName("IX_item_template_slot_item");

            builder.Property(e => e.SlotId)
                .HasColumnName("slot");

            builder.Property(e => e.ItemTemplateId)
                .HasColumnName("itemTemplate");

            builder.HasOne(s => s.ItemTemplate)
                .WithMany(s => s.Slots)
                .HasForeignKey(s => s.ItemTemplateId)
                .HasConstraintName("FK_item_template_slot_item_template_item");

            builder.HasOne(s => s.Slot)
                .WithMany()
                .HasForeignKey(s => s.SlotId)
                .HasConstraintName("FK_item_template_slot_item_slot_slot");
        }
    }

    public class ItemTemplateUnSkillConfiguration : IEntityTypeConfiguration<ItemTemplateUnSkill>
    {
        public void Configure(EntityTypeBuilder<ItemTemplateUnSkill> builder)
        {
            builder.ToTable("item_template_unskills");

            builder.HasKey(e => new {e.SkillId, e.ItemTemplateId});

            builder.HasIndex(e => e.ItemTemplateId)
                .HasName("IX_item_unskill_item");

            builder.HasIndex(e => e.SkillId)
                .HasName("IX_item_template_unskills_skillId");

            builder.Property(e => e.ItemTemplateId)
                .HasColumnName("itemTemplate");

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
                .HasConstraintName("FK_item_unskill_item_template_item");
        }
    }

    public class ItemTypeConfiguration : IEntityTypeConfiguration<ItemType>
    {
        public void Configure(EntityTypeBuilder<ItemType> builder)
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

    public class SlotConfiguration : IEntityTypeConfiguration<Slot>
    {
        public void Configure(EntityTypeBuilder<Slot> builder)
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