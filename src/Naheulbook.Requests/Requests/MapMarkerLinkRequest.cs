// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Naheulbook.Requests.Requests;

public class MapMarkerLinkRequest
{
    public string? Name { get; set; }
    public int TargetMapId { get; set; }
    public int? TargetMapMarkerId { get; set; }
}