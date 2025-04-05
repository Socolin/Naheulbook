using JetBrains.Annotations;

namespace Naheulbook.Web.Responses;

[PublicAPI]
public class FightResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public List<MonsterResponse> Monsters { get; set; } = null!;
}