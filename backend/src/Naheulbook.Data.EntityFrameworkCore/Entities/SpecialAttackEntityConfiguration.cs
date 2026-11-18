using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Naheulbook.Data.EntityFrameworkCore.Entities;

public class SpecialAttackEntityConfiguration : IEntityTypeConfiguration<SpecialAttackEntity>
{
    public void Configure(EntityTypeBuilder<SpecialAttackEntity> builder)
    {
        builder.ToTable("special_attacks");

        builder.HasKey(e => e.Id);

        builder.HasOne(x => x.SpecialAttackGroup)
            .WithMany(x => x.SpecialAttacks)
            .HasForeignKey(x => x.SpecialAttackGroupId);
    }
}
