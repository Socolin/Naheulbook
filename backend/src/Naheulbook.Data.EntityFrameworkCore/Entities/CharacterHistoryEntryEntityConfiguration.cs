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
        builder.Property(e => e.Action)
            .HasColumnName("action");
        builder.Property(e => e.Gm)
            .HasColumnName("gm");

        builder.HasOne(e => e.Character)
            .WithMany(g => g.HistoryEntries)
            .HasForeignKey(e => e.CharacterId)
            .HasConstraintName("FK_character_history_character_character");
    }
}