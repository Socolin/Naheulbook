using Naheulbook.Data.EntityFrameworkCore.Extensions;

namespace Naheulbook.Data.EntityFrameworkCore.Entities;

public class CharacterSpellBookEntity
{
    public int CharacterId { get; set; }
    private CharacterEntity? _character;
    public CharacterEntity Character { get => _character.ThrowIfNotLoaded(); set => _character = value; }

    public Guid SpellBookId { get; set; }
    private SpellBookEntity? _spellBook;
    public SpellBookEntity SpellBook { get => _spellBook.ThrowIfNotLoaded(); set => _spellBook = value; }

    public int? MaxLevel { get; set; }
}
