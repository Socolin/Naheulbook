using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Naheulbook.Data.EntityFrameworkCore.Entities;

public class CharacterHandicapEntityConfiguration : IEntityTypeConfiguration<CharacterHandicapEntity>
{
    public void Configure(EntityTypeBuilder<CharacterHandicapEntity> builder)
    {
        builder.ToTable("characters_handicaps");

        builder.HasKey(e => new {e.CharacterId, e.HandicapId});

        builder.HasOne(x => x.Character)
            .WithMany(x => x.Handicaps)
            .HasForeignKey(x => x.CharacterId);

        builder.HasOne(x => x.Handicap)
            .WithMany()
            .HasForeignKey(x => x.HandicapId);
    }
}
