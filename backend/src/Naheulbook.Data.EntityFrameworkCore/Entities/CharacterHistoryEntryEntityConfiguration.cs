using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Naheulbook.Data.EntityFrameworkCore.Entities;

public class CharacterHistoryEntryEntityConfiguration : IEntityTypeConfiguration<CharacterHistoryEntryEntity>
{
    public void Configure(EntityTypeBuilder<CharacterHistoryEntryEntity> builder)
    {
        builder.ToTable("character_history_entries");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .HasColumnName("id");
        builder.Property(e => e.CharacterId)
            .HasColumnName("character");
        builder.Property(e => e.Data)
            .HasColumnName("data")
            .HasColumnType("json");
        builder.Property(e => e.Date)
            .HasColumnName("date");
        builder.Property(e => e.Info)
            .HasColumnName("info");
        builder.Property(e => e.Action)
            .HasColumnName("action");
        builder.Property(e => e.Gm)
            .HasColumnName("gm");
        builder.Property(e => e.EffectId)
            .HasColumnName("effect");
        builder.Property(e => e.ItemId)
            .HasColumnName("item");
        builder.Property(e => e.CharacterModifierId)
            .HasColumnName("modifier");

        builder.HasOne(e => e.Character)
            .WithMany(g => g.HistoryEntries)
            .HasForeignKey(e => e.CharacterId)
            .HasConstraintName("FK_character_history_character_character");

        builder.HasOne(e => e.CharacterModifier)
            .WithMany()
            .HasForeignKey(e => e.CharacterModifierId)
            .HasConstraintName("FK_character_history_character_modifier_modifier");

        builder.HasOne(e => e.Effect)
            .WithMany()
            .HasForeignKey(e => e.EffectId)
            .HasConstraintName("FK_character_history_effect_effect");

        builder.HasOne(e => e.Item)
            .WithMany()
            .HasForeignKey(e => e.ItemId)
            .HasConstraintName("FK_character_history_item_item");
    }
}