using JetBrains.Annotations;

namespace Naheulbook.Web.Responses;

[PublicAPI]
public class EffectResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public string DurationType { get; set; } = null!;
    public string? Duration { get; set; }
    public int? CombatCount { get; set; }
    public int? LapCount { get; set; }
    public int? TimeDuration { get; set; }
    public int? Dice { get; set; }
    public int SubCategoryId { get; set; }
    public IList<StatModifierResponse> Modifiers { get; set; } = null!;
}

[PublicAPI]
public class EffectTypeResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public IList<EffectSubCategoryResponse> SubCategories { get; set; } = null!;
}

[PublicAPI]
public class EffectSubCategoryResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public short DiceCount { get; set; }
    public short DiceSize { get; set; }
    public string? Note { get; set; }
    public int TypeId { get; set; }
}