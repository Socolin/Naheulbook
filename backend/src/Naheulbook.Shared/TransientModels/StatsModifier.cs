namespace Naheulbook.Shared.TransientModels;

[Serializable]
public class StatsModifier
{
    public string Name { get; set; } = null!;

    public bool Reusable { get; set; }

    public string DurationType { get; set; } = null!;
    public string? Duration { get; set; }
    public int? CombatCount { get; set; }
    public int? LapCount { get; set; }
    public int? TimeDuration { get; set; }

    public string? Description { get; set; }
    public string? Type { get; set; }

    public List<StatModifierRequest> Values { get; set; } = null!;
}