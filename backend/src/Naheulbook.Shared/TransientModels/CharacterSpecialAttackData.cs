namespace Naheulbook.Shared.TransientModels;

public class CharacterSpecialAttackData
{
    /// <summary>
    /// Number of successful uses, used for the special attacks prerequisites (e.g. "succeed 5 times Concentration rapide I")
    /// </summary>
    public int SuccessCount { get; set; }

    /// <summary>
    /// Running cooldowns. The special attack is available again once they are expired (and removed).
    /// </summary>
    public List<SpecialAttackCooldown> Cooldowns { get; set; } = [];
}

/// <summary>
/// A running cooldown: the duration of the special attack cooldown and the remaining time, using the same fields as the modifiers
/// so the group duration updates (time added, end of combat) can decrement it.
/// </summary>
public class SpecialAttackCooldown : Durable
{
    public int? CurrentCombatCount { get; set; }
    public int? CurrentLapCount { get; set; }
    public int? CurrentTimeDuration { get; set; }

    /// <summary>
    /// True when this cooldown comes from a failed attempt
    /// </summary>
    public bool Failure { get; set; }
}
