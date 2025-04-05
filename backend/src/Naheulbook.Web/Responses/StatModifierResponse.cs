using JetBrains.Annotations;
using Newtonsoft.Json;

namespace Naheulbook.Web.Responses;

[PublicAPI]
public class StatModifierResponse
{
    public string Stat { get; set; } = null!;
    public string Type { get; set; } = null!;
    public int Value { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public List<string>? Special { get; set; }
}