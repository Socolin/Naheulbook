namespace Naheulbook.Requests.Requests;

[PublicAPI]
public class MapLayerRequest
{
    public required string Name { get; set; }
    public required string Source { get; set; }
    public bool IsGm { get; set; }
}