using Naheulbook.Data.EntityFrameworkCore.Extensions;

namespace Naheulbook.Data.EntityFrameworkCore.Entities;

public class MasteryEntity
{
    public Guid Id { get; set; }
    public int DisplayOrder { get; set; }
    public string Name { get; set; } = null!;
    public string Data { get; set; } = null!;

    public Guid MasteryGroupId { get; set; }
    private MasteryGroupEntity? _masteryGroup;
    public MasteryGroupEntity MasteryGroup { get => _masteryGroup.ThrowIfNotLoaded(); set => _masteryGroup = value; }
}
