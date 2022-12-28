using Newtonsoft.Json.Linq;

namespace Naheulbook.Requests.Requests;

[PublicAPI]
public class MapMarkerRequest
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public required string Type { get; set; }
    public required JObject MarkerInfo { get; set; }
}