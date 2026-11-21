using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Naheulbook.Data.EntityFrameworkCore.Entities;

public class SpellEntityConfiguration : IEntityTypeConfiguration<SpellEntity>
{
    public void Configure(EntityTypeBuilder<SpellEntity> builder)
    {
        builder.ToTable("spells");

        builder.HasKey(e => e.Id);

        builder.HasOne(x => x.SpellBook)
            .WithMany(x => x.Spells)
            .HasForeignKey(x => x.SpellBookId);
    }
}
