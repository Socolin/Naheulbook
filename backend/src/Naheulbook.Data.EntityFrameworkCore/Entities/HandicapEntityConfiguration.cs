using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Naheulbook.Data.EntityFrameworkCore.Entities;

public class HandicapEntityConfiguration : IEntityTypeConfiguration<HandicapEntity>
{
    public void Configure(EntityTypeBuilder<HandicapEntity> builder)
    {
        builder.ToTable("handicaps");

        builder.HasKey(x => x.Id);
    }
}
