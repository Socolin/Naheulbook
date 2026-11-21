using Naheulbook.Data.EntityFrameworkCore.Extensions;

namespace Naheulbook.Data.EntityFrameworkCore.Entities;

public class SpellEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public int? Level { get; set; }
    public string? LevelLabel { get; set; }
    public string Kind { get; set; } = null!;
    public string? Category { get; set; }
    public int DisplayOrder { get; set; }
    public string Description { get; set; } = null!;
    public string Data { get; set; } = null!;

    public Guid SpellBookId { get; set; }
    private SpellBookEntity? _spellBook;
    public SpellBookEntity SpellBook { get => _spellBook.ThrowIfNotLoaded(); set => _spellBook = value; }
}
