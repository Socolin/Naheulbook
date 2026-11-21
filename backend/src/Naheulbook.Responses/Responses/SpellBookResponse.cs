using JetBrains.Annotations;
using Naheulbook.Shared.TransientModels;

namespace Naheulbook.Responses.Responses;

[PublicAPI]
public class SpellBooksResponse
{
    public IReadOnlyCollection<SpellBookResponse> SpellBooks { get; set; } = [];
}

[PublicAPI]
public class SpellBookResponse
{
    public Guid Id { get; set; }
    public string TechName { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string Kind { get; set; } = null!;
    public Guid? JobId { get; set; }
    public Guid? SpecialityId { get; set; }
    public int? GodId { get; set; }
    public SpellBookData Data { get; set; } = null!;
    public IReadOnlyCollection<SpellResponse> Spells { get; set; } = [];
}
