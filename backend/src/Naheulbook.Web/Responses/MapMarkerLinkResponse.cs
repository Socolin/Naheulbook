using JetBrains.Annotations;

namespace Naheulbook.Web.Responses;

[PublicAPI]
public class MapMarkerLinkResponse
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string TargetMapName { get; set; } = null!;
    public int TargetMapId { get; set; }
    public bool TargetMapIsGm { get; set; }
    public int? TargetMapMarkerId { get; set; }
}