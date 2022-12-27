using Newtonsoft.Json.Linq;

// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Naheulbook.Requests.Requests;

public class MapMarkerRequest
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public string Type { get; set; } = null!;
    public JObject MarkerInfo { get; set; } = null!;
}