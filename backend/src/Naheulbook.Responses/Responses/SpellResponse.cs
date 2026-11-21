using JetBrains.Annotations;
using Naheulbook.Shared.TransientModels;

namespace Naheulbook.Responses.Responses;

[PublicAPI]
public class SpellResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public int? Level { get; set; }
    public string? LevelLabel { get; set; }
    public string Kind { get; set; } = null!;
    public string? Category { get; set; }
    public string Description { get; set; } = null!;
    public SpellData Data { get; set; } = null!;
}
