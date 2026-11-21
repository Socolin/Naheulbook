using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Naheulbook.Data.EntityFrameworkCore.Entities;

public class DebilibeukBlessingEntityConfiguration : IEntityTypeConfiguration<DebilibeukBlessingEntity>
{
    public void Configure(EntityTypeBuilder<DebilibeukBlessingEntity> builder)
    {
        builder.ToTable("debilibeuk_blessings");

        builder.HasKey(x => x.Id);
    }
}
