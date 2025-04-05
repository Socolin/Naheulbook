using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Naheulbook.Data.EntityFrameworkCore.Entities;

public class CharacterSkillEntityConfiguration : IEntityTypeConfiguration<CharacterSkillEntity>
{
    public void Configure(EntityTypeBuilder<CharacterSkillEntity> builder)
    {
        builder.ToTable("character_skills");

        builder.HasKey(e => new {e.SkillId, e.CharacterId});

        builder.HasIndex(e => e.CharacterId)
            .HasDatabaseName("IX_character_skills_characterId");

        builder.HasIndex(e => e.SkillId)
            .HasDatabaseName("IX_character_skills_skillId");

        builder.Property(e => e.CharacterId)
            .HasColumnName("characterId");

        builder.Property(e => e.SkillId)
            .HasColumnName("skillId");

        builder.HasOne(e => e.Character)
            .WithMany(e => e.Skills)
            .HasForeignKey(e => e.CharacterId)
            .HasConstraintName("FK_character_skills_characterId_characters_id");

        builder.HasOne(e => e.Skill)
            .WithMany()
            .HasForeignKey(e => e.SkillId)
            .HasConstraintName("FK_character_skills_skillId_skills_id");
    }
}