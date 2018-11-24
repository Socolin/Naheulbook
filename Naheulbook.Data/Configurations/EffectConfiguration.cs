using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Naheulbook.Data.Models;

namespace Naheulbook.Data.Configurations
{
    public class EffectConfiguration : IEntityTypeConfiguration<Effect>
    {
        public void Configure(EntityTypeBuilder<Effect> builder)
        {
            builder.ToTable("effect");

            builder.HasIndex(e => e.CategoryId)
                .HasName("IX_effect_category");

            builder.Property(e => e.Id)
                .HasColumnName("id");

            builder.Property(e => e.CategoryId)
                .HasColumnName("category");

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

            builder.HasOne(e => e.Category)
                .WithMany(e => e.Effects)
                .OnDelete(DeleteBehavior.Restrict)
                .HasForeignKey(e => e.CategoryId)
                .HasConstraintName("FK_effect_effect_category_category");
        }
    }

    public class EffectCategoryConfiguration : IEntityTypeConfiguration<EffectCategory>
    {
        public void Configure(EntityTypeBuilder<EffectCategory> builder)
        {
            builder.ToTable("effect_category");

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
                .IsRequired()
                .HasColumnName("note");

            builder.HasOne(d => d.Type)
                .WithMany(p => p.Categories)
                .HasForeignKey(d => d.TypeId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("effect_category_effect_type_id_fk");
        }
    }

    public class EffectModifierConfiguration : IEntityTypeConfiguration<EffectModifier>
    {
        public void Configure(EntityTypeBuilder<EffectModifier> builder)
        {
            builder.HasKey(e => new {e.EffectId, e.StatName});

            builder.ToTable("effect_modifier");

            builder.HasIndex(e => e.StatName)
                .HasName("IX_effect_modifier_stat");

            builder.Property(e => e.EffectId)
                .HasColumnName("effect");

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
                .HasConstraintName("FK_effect_modifier_effect_effect");

            builder.HasOne(e => e.Stat)
                .WithMany(e => e.Effects)
                .HasForeignKey(e => e.StatName)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_effect_modifier_stat_stat");
        }
    }

    public class EffectTypeConfiguration : IEntityTypeConfiguration<EffectType>
    {
        public void Configure(EntityTypeBuilder<EffectType> builder)
        {
            builder.ToTable("effect_type");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.Id)
                .HasColumnName("id");

            builder.Property(e => e.Name)
                .IsRequired()
                .HasColumnName("name")
                .HasMaxLength(255);
        }
    }
}