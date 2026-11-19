using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Naheulbook.Data.EntityFrameworkCore.Entities;

public class MasteryEntityConfiguration : IEntityTypeConfiguration<MasteryEntity>
{
    public void Configure(EntityTypeBuilder<MasteryEntity> builder)
    {
        builder.ToTable("masteries");

        builder.HasKey(e => e.Id);

        builder.HasOne(x => x.MasteryGroup)
            .WithMany(x => x.Masteries)
            .HasForeignKey(x => x.MasteryGroupId);
    }
}
