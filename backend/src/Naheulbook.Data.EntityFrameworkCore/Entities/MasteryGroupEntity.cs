using Naheulbook.Data.EntityFrameworkCore.Extensions;

namespace Naheulbook.Data.EntityFrameworkCore.Entities;

[Serializable]
public class MasteryGroupEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public int DisplayOrder { get; set; }

    private ICollection<MasteryEntity>? _masteries;
    public ICollection<MasteryEntity> Masteries { get => _masteries.ThrowIfNotLoaded(); set => _masteries = value; }
}
