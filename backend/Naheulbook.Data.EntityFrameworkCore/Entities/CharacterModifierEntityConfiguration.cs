using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Naheulbook.Data.EntityFrameworkCore.Entities;

public class CharacterModifierEntityConfiguration : IEntityTypeConfiguration<CharacterModifierEntity>
{
    public void Configure(EntityTypeBuilder<CharacterModifierEntity> builder)
    {
        builder.ToTable("character_modifiers");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .HasColumnName("id");

        builder.Property(e => e.Name)
            .HasColumnName("name");
        builder.Property(e => e.Type)
            .HasColumnName("type");
        builder.Property(e => e.Description)
            .HasColumnName("description");

        builder.Property(e => e.IsActive)
            .HasColumnName("active");
        builder.Property(e => e.Permanent)
            .HasColumnName("permanent");
        builder.Property(e => e.Reusable)
            .HasColumnName("reusable");

        builder.Property(e => e.DurationType)
            .HasColumnName("durationtype");

        builder.Property(e => e.TimeDuration)
            .IsRequired(false)
            .HasColumnName("timeduration");
        builder.Property(e => e.LapCount)
            .IsRequired(false)
            .HasColumnName("lapcount");
        builder.Property(e => e.CombatCount)
            .IsRequired(false)
            .HasColumnName("combatcount");
        builder.Property(e => e.LapCountDecrement)
            .IsRequired(false)
            .HasColumnName("lapCountDecrement");

        builder.Property(e => e.CurrentLapCount)
            .IsRequired(false)
            .HasColumnName("currentlapcount");
        builder.Property(e => e.CurrentCombatCount)
            .IsRequired(false)
            .HasColumnName("currentcombatcount");
        builder.Property(e => e.CurrentTimeDuration)
            .IsRequired(false)
            .HasColumnName("currenttimeduration");
        builder.Property(e => e.Duration)
            .IsRequired(false)
            .HasColumnName("duration");

        builder.Property(e => e.CharacterId)
            .HasColumnName("characterId");

        builder.HasOne(e => e.Character!)
            .WithMany(e => e.Modifiers)
            .HasForeignKey(e => e.CharacterId)
            .HasConstraintName("FK_character_modifiers_characterId_characters_id");

        builder.HasMany(e => e.Values)
            .WithOne()
            .HasForeignKey(e => e.CharacterModifierId)
            .HasConstraintName("FK_character_modifier_values_characterModifierId");
    }
}