using JetBrains.Annotations;

namespace Naheulbook.Responses.Responses;

[PublicAPI]
public class AptitudeGroupsResponse
{
    public IReadOnlyCollection<SummaryAptitudeGroupResponse> AptitudeGroups { get; set; } = [];
}

[PublicAPI]
public class SummaryAptitudeGroupResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
}

[PublicAPI]
public class AptitudeGroupResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public IReadOnlyCollection<AptitudeResponse> Aptitudes { get; set; } = [];
}