using Newtonsoft.Json;

namespace Naheulbook.Requests.Requests;

[PublicAPI]
public class PatchCharacterRequest
{
    public int? Ev { get; set; }
    public int? Ea { get; set; }
    public short? FatePoint { get; set; }
    public int? Experience { get; set; }
    public string? Sex { get; set; }
    public string? Name { get; set; }
    public string? Notes { get; set; }

    [JsonProperty("active")]
    public bool? IsActive { get; set; }

    public int? OwnerId { get; set; }
    public TargetRequest? Target { get; set; }
    public string? Color { get; set; }
    public int? Mankdebol { get; set; }
    public int? Debilibeuk { get; set; }
}