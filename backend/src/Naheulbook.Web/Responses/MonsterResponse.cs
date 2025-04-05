using JetBrains.Annotations;
using Naheulbook.Shared.TransientModels;
using Newtonsoft.Json.Linq;

namespace Naheulbook.Web.Responses;

[PublicAPI]
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