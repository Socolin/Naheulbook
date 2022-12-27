// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Naheulbook.Requests.Requests;

public class MapLayerRequest
{
    public string Name { get; set; } = null!;
    public string Source { get; set; } = null!;
    public bool IsGm { get; set; }
}