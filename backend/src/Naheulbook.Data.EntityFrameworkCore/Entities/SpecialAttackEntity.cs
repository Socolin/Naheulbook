using Naheulbook.Data.EntityFrameworkCore.Extensions;

namespace Naheulbook.Data.EntityFrameworkCore.Entities;

public class SpecialAttackEntity
{
    public Guid Id { get; set; }
    public int DisplayOrder { get; set; }
    public string Name { get; set; } = null!;
    public string Data { get; set; } = null!;

    public Guid SpecialAttackGroupId { get; set; }
    private SpecialAttackGroupEntity? _specialAttackGroup;
    public SpecialAttackGroupEntity SpecialAttackGroup { get => _specialAttackGroup.ThrowIfNotLoaded(); set => _specialAttackGroup = value; }
}
