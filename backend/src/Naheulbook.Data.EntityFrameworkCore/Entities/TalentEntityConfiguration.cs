using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Naheulbook.Data.EntityFrameworkCore.Entities;

public class TalentEntityConfiguration : IEntityTypeConfiguration<TalentEntity>
{
    public void Configure(EntityTypeBuilder<TalentEntity> builder)
    {
        builder.ToTable("talents");

        builder.HasKey(e => e.Id);

        builder.HasOne(x => x.TalentGroup)
            .WithMany(x => x.Talents)
            .HasForeignKey(x => x.TalentGroupId);
    }
}
