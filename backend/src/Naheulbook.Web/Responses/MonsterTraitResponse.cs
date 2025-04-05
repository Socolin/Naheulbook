using JetBrains.Annotations;

namespace Naheulbook.Web.Responses;

[PublicAPI]
public class MonsterTraitResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public IList<string>? Levels { get; set; }
}