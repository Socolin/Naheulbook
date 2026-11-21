using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Naheulbook.Data.EntityFrameworkCore.Entities;

public class SpellBookEntityConfiguration : IEntityTypeConfiguration<SpellBookEntity>
{
    public void Configure(EntityTypeBuilder<SpellBookEntity> builder)
    {
        builder.ToTable("spell_books");

        builder.HasKey(x => x.Id);
    }
}
