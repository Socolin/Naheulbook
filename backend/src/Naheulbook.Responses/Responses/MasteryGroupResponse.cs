using JetBrains.Annotations;

namespace Naheulbook.Responses.Responses;

[PublicAPI]
public class MasteryGroupsResponse
{
    public IReadOnlyCollection<MasteryGroupResponse> MasteryGroups { get; set; } = [];
}

[PublicAPI]
public class MasteryGroupResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public IReadOnlyCollection<MasteryResponse> Masteries { get; set; } = [];
}
