using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Naheulbook.Data.EntityFrameworkCore.Entities;

public class CharacterSpellBookEntityConfiguration : IEntityTypeConfiguration<CharacterSpellBookEntity>
{
    public void Configure(EntityTypeBuilder<CharacterSpellBookEntity> builder)
    {
        builder.ToTable("characters_spell_books");

        builder.HasKey(e => new {e.CharacterId, e.SpellBookId});

        builder.HasOne(x => x.Character)
            .WithMany(x => x.SpellBooks)
            .HasForeignKey(x => x.CharacterId);

        builder.HasOne(x => x.SpellBook)
            .WithMany()
            .HasForeignKey(x => x.SpellBookId);
    }
}
