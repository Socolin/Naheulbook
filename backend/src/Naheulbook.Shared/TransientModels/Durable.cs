namespace Naheulbook.Shared.TransientModels;

[Serializable]
public class Durable
{
    public string DurationType { get; set; } = null!;
    public int? CombatCount { get; set; }
    public int? LapCount { get; set; }
    public string? Duration { get; set; }
    public int? TimeDuration { get; set; }
}