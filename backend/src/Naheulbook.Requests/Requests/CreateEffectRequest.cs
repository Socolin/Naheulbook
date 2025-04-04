using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Naheulbook.Shared.TransientModels;

namespace Naheulbook.Requests.Requests;

[PublicAPI]
public class CreateEffectRequest
{
    [StringLength(255, MinimumLength = 1)]
    public required string Name { get; set; }

    public string? Description { get; set; }
    public required string DurationType { get; set; }
    public int? CombatCount { get; set; }
    public string? Duration { get; set; }
    public short? Dice { get; set; }
    public int? LapCount { get; set; }
    public int? TimeDuration { get; set; }
    public required List<StatModifierRequest> Modifiers { get; set; }
}