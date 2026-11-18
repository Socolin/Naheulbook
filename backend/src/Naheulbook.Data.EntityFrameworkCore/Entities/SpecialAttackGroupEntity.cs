using Naheulbook.Data.EntityFrameworkCore.Extensions;

namespace Naheulbook.Data.EntityFrameworkCore.Entities;

[Serializable]
public class SpecialAttackGroupEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public int DisplayOrder { get; set; }

    private ICollection<SpecialAttackEntity>? _specialAttacks;
    public ICollection<SpecialAttackEntity> SpecialAttacks { get => _specialAttacks.ThrowIfNotLoaded(); set => _specialAttacks = value; }
}
