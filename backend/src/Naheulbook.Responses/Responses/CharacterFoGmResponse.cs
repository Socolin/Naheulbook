using JetBrains.Annotations;
using Newtonsoft.Json;
using Naheulbook.Shared.TransientModels;

namespace Naheulbook.Responses.Responses;

[PublicAPI]
public class CharacterFoGmResponse : CharacterResponse
{
    [JsonProperty("active")]
    public bool IsActive { get; set; }

    public string Color { get; set; } = null!;
    public CharacterGmData? GmData { get; set; }
    public int? OwnerId { get; set; }
    public TargetResponse? Target { get; set; }
}