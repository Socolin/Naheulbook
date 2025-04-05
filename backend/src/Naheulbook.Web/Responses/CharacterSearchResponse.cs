using JetBrains.Annotations;
using Newtonsoft.Json;

namespace Naheulbook.Web.Responses;

[PublicAPI]
public class CharacterSearchResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;

    [JsonProperty("origin")]
    public string OriginName { get; set; } = null!;

    [JsonProperty("owner")]
    public string OwnerName { get; set; } = null!;
}