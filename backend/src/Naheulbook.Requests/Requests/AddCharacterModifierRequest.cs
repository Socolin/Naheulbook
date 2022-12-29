using System.Collections.Generic;
using Naheulbook.Shared.TransientModels;

namespace Naheulbook.Requests.Requests;

[PublicAPI]
public class AddCharacterModifierRequest
{
    public required string Name { get; set; }

    public bool Reusable { get; set; }

    public required string DurationType { get; set; }
    public string? Duration { get; set; }
    public int? CombatCount { get; set; }
    public int? LapCount { get; set; }
    public int? TimeDuration { get; set; }

    public string? Description { get; set; }
    public string? Type { get; set; }

    public List<StatModifierRequest> Values { get; set; } = new();

    public LapCountDecrement? LapCountDecrement { get; set; }
}