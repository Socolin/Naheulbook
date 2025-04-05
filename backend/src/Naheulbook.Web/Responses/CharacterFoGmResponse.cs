using JetBrains.Annotations;
using Naheulbook.Core.Features.Character;
using Newtonsoft.Json;

namespace Naheulbook.Web.Responses;

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