using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Naheulbook.Data.EntityFrameworkCore.Entities;

public class CharacterMasteryEntityConfiguration : IEntityTypeConfiguration<CharacterMasteryEntity>
{
    public void Configure(EntityTypeBuilder<CharacterMasteryEntity> builder)
    {
        builder.ToTable("characters_masteries");

        builder.HasKey(e => new {e.CharacterId, e.MasteryId});

        builder.HasOne(x => x.Character)
            .WithMany(x => x.Masteries)
            .HasForeignKey(x => x.CharacterId);

        builder.HasOne(x => x.Mastery)
            .WithMany()
            .HasForeignKey(x => x.MasteryId);
    }
}
