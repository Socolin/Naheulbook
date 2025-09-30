using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Naheulbook.Data.EntityFrameworkCore.Entities;

public class AptitudeGroupEntityConfiguration : IEntityTypeConfiguration<AptitudeGroupEntity>
{
    public void Configure(EntityTypeBuilder<AptitudeGroupEntity> builder)
    {
        builder.ToTable("aptitude_groups");

        builder.HasKey(x => x.Id);
    }
}