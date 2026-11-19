using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Naheulbook.Data.EntityFrameworkCore.Entities;

public class TalentGroupEntityConfiguration : IEntityTypeConfiguration<TalentGroupEntity>
{
    public void Configure(EntityTypeBuilder<TalentGroupEntity> builder)
    {
        builder.ToTable("talent_groups");

        builder.HasKey(x => x.Id);
    }
}
