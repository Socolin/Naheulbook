using JetBrains.Annotations;

namespace Naheulbook.Responses.Responses;

[PublicAPI]
public class TalentGroupsResponse
{
    public IReadOnlyCollection<TalentGroupResponse> TalentGroups { get; set; } = [];
}

[PublicAPI]
public class TalentGroupResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public IReadOnlyCollection<TalentResponse> Talents { get; set; } = [];
}
