using Newtonsoft.Json;

// ReSharper disable UnusedMember.Global

namespace Naheulbook.Web.Responses;

public class CharacterSearchResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;

    [JsonProperty("origin")]
    public string OriginName { get; set; } = null!;

    [JsonProperty("owner")]
    public string OwnerName { get; set; } = null!;
}