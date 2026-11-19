using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Naheulbook.Data.EntityFrameworkCore.Entities;

public class CharacterTalentEntityConfiguration : IEntityTypeConfiguration<CharacterTalentEntity>
{
    public void Configure(EntityTypeBuilder<CharacterTalentEntity> builder)
    {
        builder.ToTable("characters_talents");

        builder.HasKey(e => new {e.CharacterId, e.TalentId});

        builder.HasOne(x => x.Character)
            .WithMany(x => x.Talents)
            .HasForeignKey(x => x.CharacterId);

        builder.HasOne(x => x.Talent)
            .WithMany()
            .HasForeignKey(x => x.TalentId);
    }
}
