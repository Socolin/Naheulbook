using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Naheulbook.Data.EntityFrameworkCore.Entities;

public class MankdebolCalamityEntityConfiguration : IEntityTypeConfiguration<MankdebolCalamityEntity>
{
    public void Configure(EntityTypeBuilder<MankdebolCalamityEntity> builder)
    {
        builder.ToTable("mankdebol_calamities");

        builder.HasKey(x => x.Id);
    }
}
