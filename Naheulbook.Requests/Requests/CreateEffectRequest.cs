using System.Collections.Generic;
using Naheulbook.Shared.TransientModels;

// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable UnusedMember.Global

namespace Naheulbook.Requests.Requests;

public class CreateEffectRequest
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public string DurationType { get; set; } = null!;
    public int? CombatCount { get; set; }
    public string? Duration { get; set; }
    public short? Dice { get; set; }
    public int? LapCount { get; set; }
    public int? TimeDuration { get; set; }
    public List<StatModifier> Modifiers { get; set; } = null!;
}