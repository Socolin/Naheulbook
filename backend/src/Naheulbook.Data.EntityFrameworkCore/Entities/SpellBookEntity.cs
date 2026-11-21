using Naheulbook.Data.EntityFrameworkCore.Extensions;

namespace Naheulbook.Data.EntityFrameworkCore.Entities;

[Serializable]
public class SpellBookEntity
{
    public Guid Id { get; set; }
    public string TechName { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string Kind { get; set; } = null!;
    public Guid? JobId { get; set; }
    public Guid? SpecialityId { get; set; }
    public int? GodId { get; set; }
    public int DisplayOrder { get; set; }
    public string Data { get; set; } = null!;

    private ICollection<SpellEntity>? _spells;
    public ICollection<SpellEntity> Spells { get => _spells.ThrowIfNotLoaded(); set => _spells = value; }
}
