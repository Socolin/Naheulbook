using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Naheulbook.Data.EntityFrameworkCore.Entities;

public class MasteryGroupEntityConfiguration : IEntityTypeConfiguration<MasteryGroupEntity>
{
    public void Configure(EntityTypeBuilder<MasteryGroupEntity> builder)
    {
        builder.ToTable("mastery_groups");

        builder.HasKey(x => x.Id);
    }
}
