using JetBrains.Annotations;
using Newtonsoft.Json;

namespace Naheulbook.Web.Responses;

[PublicAPI]
public class CharacterSummaryResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public int Level { get; set; }

    [JsonProperty("origin")]
    public string OriginName { get; set; } = null!;

    [JsonProperty("jobs")]
    public IList<string> JobNames { get; set; } = null!;
}