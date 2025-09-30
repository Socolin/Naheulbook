using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Naheulbook.Data.EntityFrameworkCore.Entities;

public class AptitudeEntityConfiguration : IEntityTypeConfiguration<AptitudeEntity>
{
    public void Configure(EntityTypeBuilder<AptitudeEntity> builder)
    {
        builder.ToTable("aptitudes");

        builder.HasKey(e => e.Id);

        builder.HasOne(x => x.AptitudeGroup)
            .WithMany(x => x.Aptitudes)
            .HasForeignKey(x => x.AptitudeGroupId);
    }
}