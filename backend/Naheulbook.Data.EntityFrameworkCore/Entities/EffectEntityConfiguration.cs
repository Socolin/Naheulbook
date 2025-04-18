using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Naheulbook.Data.EntityFrameworkCore.Entities;

public class EffectEntityConfiguration : IEntityTypeConfiguration<EffectEntity>
{
    public void Configure(EntityTypeBuilder<EffectEntity> builder)
    {
        builder.ToTable("effects");

        builder.HasIndex(e => e.SubCategoryId)
            .HasDatabaseName("IX_effect_subCategoryId");

        builder.Property(e => e.Id)
            .HasColumnName("id");

        builder.Property(e => e.SubCategoryId)
            .HasColumnName("subCategoryId");

        builder.Property(e => e.DurationType)
            .HasColumnName("durationtype");

        builder.Property(e => e.CombatCount)
            .HasColumnName("combatcount");

        builder.Property(e => e.LapCount)
            .HasColumnName("lapcount");

        builder.Property(e => e.Description)
            .HasColumnName("description");

        builder.Property(e => e.Dice)
            .HasColumnName("dice");

        builder.Property(e => e.Duration)
            .HasColumnName("duration");

        builder.Property(e => e.Name)
            .IsRequired()
            .HasColumnName("name")
            .HasMaxLength(255);

        builder.Property(e => e.TimeDuration)
            .HasColumnName("timeduration");

        builder.HasOne(e => e.SubCategory)
            .WithMany(e => e.Effects)
            .OnDelete(DeleteBehavior.Restrict)
            .HasForeignKey(e => e.SubCategoryId)
            .HasConstraintName("FK_effect_effect_category_category");
    }
}

public class EffectSubCategoryEntityConfiguration : IEntityTypeConfiguration<EffectSubCategoryEntity>
{
    public void Configure(EntityTypeBuilder<EffectSubCategoryEntity> builder)
    {
        builder.ToTable("effect_subcategories");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .HasColumnName("id");

        builder.Property(e => e.DiceCount)
            .HasColumnName("dicecount");

        builder.Property(e => e.DiceSize)
            .HasColumnName("dicesize");

        builder.Property(e => e.TypeId)
            .HasColumnName("typeid");

        builder.Property(e => e.Name)
            .IsRequired()
            .HasColumnName("name")
            .HasMaxLength(255);

        builder.Property(e => e.Note)
            .IsRequired(false)
            .HasColumnName("note");

        builder.HasOne(d => d.Type)
            .WithMany(p => p.SubCategories)
            .HasForeignKey(d => d.TypeId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("effect_category_effect_type_id_fk");
    }
}

public class EffectModifierEntityConfiguration : IEntityTypeConfiguration<EffectModifierEntity>
{
    public void Configure(EntityTypeBuilder<EffectModifierEntity> builder)
    {
        builder.HasKey(e => new {e.EffectId, e.StatName});

        builder.ToTable("effect_modifiers");

        builder.HasIndex(e => e.EffectId)
            .HasDatabaseName("IX_effect_modifiers_effectId");
        builder.HasIndex(e => e.StatName)
            .HasDatabaseName("IX_effect_modifier_stat");

        builder.Property(e => e.EffectId)
            .HasColumnName("effectId");

        builder.Property(e => e.StatName)
            .HasColumnName("stat")
            .HasMaxLength(64);

        builder.Property(e => e.Type)
            .IsRequired()
            .HasColumnName("type")
            .HasMaxLength(16)
            .HasDefaultValueSql("'ADD'");

        builder.Property(e => e.Value)
            .HasColumnName("value");

        builder.HasOne(e => e.Effect)
            .WithMany(e => e.Modifiers)
            .HasForeignKey(e => e.EffectId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_effect_modifiers_effectId_effects_id");

        builder.HasOne(e => e.Stat)
            .WithMany(e => e.Effects)
            .HasForeignKey(e => e.StatName)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_effect_modifier_stat_stat");
    }
}

public class EffectTypeEntityConfiguration : IEntityTypeConfiguration<EffectTypeEntity>
{
    public void Configure(EntityTypeBuilder<EffectTypeEntity> builder)
    {
        builder.ToTable("effect_types");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .HasColumnName("id");

        builder.Property(e => e.Name)
            .IsRequired()
            .HasColumnName("name")
            .HasMaxLength(255);
    }
}