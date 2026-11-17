using Newtonsoft.Json;

namespace Naheulbook.Responses.Responses;

public class GroupGroupInviteResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;

    [JsonProperty("origin")]
    public string OriginName { get; set; } = null!;

    [JsonProperty("jobs")]
    public IList<string> JobNames { get; set; } = null!;

    public bool FromGroup { get; set; }
}