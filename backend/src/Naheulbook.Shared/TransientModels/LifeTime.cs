namespace Naheulbook.Shared.TransientModels;

[Serializable]
public class LifeTime
{
    public DurationType DurationType { get; set; }
    public int? CombatCount { get; set; }
    public int? LapCount { get; set; }
    public string? Duration { get; set; }
    public int? TimeDuration { get; set; }
}