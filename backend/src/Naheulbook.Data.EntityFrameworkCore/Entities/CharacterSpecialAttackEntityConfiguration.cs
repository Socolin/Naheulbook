using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Naheulbook.Data.EntityFrameworkCore.Entities;

public class CharacterSpecialAttackEntityConfiguration : IEntityTypeConfiguration<CharacterSpecialAttackEntity>
{
    public void Configure(EntityTypeBuilder<CharacterSpecialAttackEntity> builder)
    {
        builder.ToTable("characters_special_attacks");

        builder.HasKey(e => new {e.CharacterId, e.SpecialAttackId});

        builder.HasOne(x => x.Character)
            .WithMany(x => x.SpecialAttacks)
            .HasForeignKey(x => x.CharacterId);

        builder.HasOne(x => x.SpecialAttack)
            .WithMany()
            .HasForeignKey(x => x.SpecialAttackId);
    }
}
