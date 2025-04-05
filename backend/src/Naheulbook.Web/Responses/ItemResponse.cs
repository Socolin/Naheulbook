using JetBrains.Annotations;
using Naheulbook.Shared.TransientModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Naheulbook.Web.Responses;

[PublicAPI]
public class ItemResponse
{
    public int Id { get; set; }
    public JObject Data { get; set; } = null!;
    public List<ActiveStatsModifier> Modifiers { get; set; } = null!;
    public int? ContainerId { get; set; }

    [JsonProperty("template")]
    public ItemTemplateResponse ItemTemplate { get; set; } = null!;
}