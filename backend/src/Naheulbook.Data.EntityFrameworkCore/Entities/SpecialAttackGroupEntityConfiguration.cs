using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Naheulbook.Data.EntityFrameworkCore.Entities;

public class SpecialAttackGroupEntityConfiguration : IEntityTypeConfiguration<SpecialAttackGroupEntity>
{
    public void Configure(EntityTypeBuilder<SpecialAttackGroupEntity> builder)
    {
        builder.ToTable("special_attack_groups");

        builder.HasKey(x => x.Id);
    }
}
