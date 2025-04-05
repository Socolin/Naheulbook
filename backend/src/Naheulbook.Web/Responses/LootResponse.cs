using JetBrains.Annotations;
using Newtonsoft.Json;

namespace Naheulbook.Web.Responses;

[PublicAPI]
public class LootResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;

    [JsonProperty("visibleForPlayer")]
    public bool IsVisibleForPlayer { get; set; }

    public List<ItemResponse> Items { get; set; } = null!;
    public List<MonsterResponse> Monsters { get; set; } = null!;
}