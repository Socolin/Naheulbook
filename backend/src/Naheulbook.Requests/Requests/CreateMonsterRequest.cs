using Naheulbook.Shared.TransientModels;

namespace Naheulbook.Requests.Requests;

[PublicAPI]
public class CreateMonsterRequest
{
    public required string Name { get; set; }
    public int? FightId { get; set; }
    public MonsterData? Data { get; set; }
    public IList<ActiveStatsModifier>? Modifiers { get; set; }
    public required IList<CreateItemRequest> Items { get; set; }
}