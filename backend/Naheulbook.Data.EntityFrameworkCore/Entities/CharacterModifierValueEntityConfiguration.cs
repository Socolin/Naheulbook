using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Naheulbook.Data.EntityFrameworkCore.Entities;

public class CharacterModifierValueEntityConfiguration : IEntityTypeConfiguration<CharacterModifierValueEntity>
{
    public void Configure(EntityTypeBuilder<CharacterModifierValueEntity> builder)
    {
        builder.ToTable("character_modifier_values");

        builder.HasKey(e => e.Id);

        builder.HasIndex(e => e.StatName)
            .HasDatabaseName("IX_character_modifier_value_stat");
        builder.HasIndex(e => e.CharacterModifierId)
            .HasDatabaseName("IX_character_modifier_values_characterModifierId");

        builder.Property(e => e.StatName)
            .HasColumnName("stat");
        builder.Property(e => e.Type)
            .HasColumnName("type");
        builder.Property(e => e.Value)
            .HasColumnName("value");
        builder.Property(e => e.CharacterModifierId)
            .HasColumnName("characterModifierId");
    }
}