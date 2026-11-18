using JetBrains.Annotations;

namespace Naheulbook.Responses.Responses;

[PublicAPI]
public class SpecialAttackGroupsResponse
{
    public IReadOnlyCollection<SpecialAttackGroupResponse> SpecialAttackGroups { get; set; } = [];
}

[PublicAPI]
public class SpecialAttackGroupResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public IReadOnlyCollection<SpecialAttackResponse> SpecialAttacks { get; set; } = [];
}
