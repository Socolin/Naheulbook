using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Naheulbook.Data.EntityFrameworkCore.Entities;

public class CharacterSpecialityEntityConfiguration : IEntityTypeConfiguration<CharacterSpecialityEntity>
{
    public void Configure(EntityTypeBuilder<CharacterSpecialityEntity> builder)
    {
        builder.ToTable("character_specialities");

        builder.HasKey(e => new {e.SpecialityId, e.CharacterId});

        builder.HasIndex(e => e.SpecialityId)
            .HasDatabaseName("IX_character_specialities_specialityId");
        builder.HasIndex(e => e.CharacterId)
            .HasDatabaseName("IX_character_specialities_characterId");

        builder.Property(e => e.CharacterId)
            .HasColumnName("characterId");
        builder.Property(e => e.SpecialityId)
            .HasColumnName("specialityId");

        builder.HasOne(e => e.Character)
            .WithMany(e => e.Specialities)
            .HasForeignKey(e => e.CharacterId)
            .HasConstraintName("FK_character_specialities_characterId_characters_id");

        builder.HasOne(e => e.Speciality)
            .WithMany()
            .HasForeignKey(e => e.SpecialityId)
            .HasConstraintName("FK_character_specialities_specialityId_specialities_id");
    }
}