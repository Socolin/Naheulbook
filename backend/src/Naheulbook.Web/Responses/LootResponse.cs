using System.Collections.Generic;
using Newtonsoft.Json;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Naheulbook.Web.Responses;

public class LootResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;

    [JsonProperty("visibleForPlayer")]
    public bool IsVisibleForPlayer { get; set; }

    public List<ItemResponse> Items { get; set; } = null!;
    public List<MonsterResponse> Monsters { get; set; } = null!;
}