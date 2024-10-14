using System.Collections.Generic;
using Naheulbook.Shared.TransientModels;
using Newtonsoft.Json.Linq;

// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable UnusedMember.Global

namespace Naheulbook.Web.Responses;

public class MonsterResponse
{
    public int Id { get; set; }
    public int? FightId { get; set; }
    public string Name { get; set; } = null!;
    public string? Dead { get; set; }
    public JObject? Data { get; set; }
    public IList<ActiveStatsModifier> Modifiers { get; set; } = null!;
    public IList<ItemResponse> Items { get; set; } = null!;
}