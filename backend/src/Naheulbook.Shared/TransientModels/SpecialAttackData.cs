namespace Naheulbook.Shared.TransientModels;

public class SpecialAttackData
{
    public int? RequiredLevel { get; set; }
    public string? UsableBy { get; set; }
    public string? Learning { get; set; }
    public string? Usage { get; set; }
    public string? Effects { get; set; }
    public string? Test { get; set; }
    public string? Conditions { get; set; }
    public string? Damage { get; set; }
    public string? CriticalFailure { get; set; }
    public string? CriticalSuccess { get; set; }
    public string? CriticalNotes { get; set; }
    public string? Range { get; set; }
    public int? Xp { get; set; }

    /// <summary>
    /// Delay before the special attack can be used again after a use. Null when the usage is not limited.
    /// </summary>
    public Durable? Cooldown { get; set; }

    /// <summary>
    /// Delay before a new attempt after a failure. Null when the special attack cannot fail.
    /// </summary>
    public Durable? FailureCooldown { get; set; }

    /// <summary>
    /// Number of uses allowed during one <see cref="Cooldown"/> period (e.g. "twice per day"). Null means 1.
    /// </summary>
    public int? UsesPerCooldown { get; set; }

    public SpecialAttackPrerequisite? Prerequisite { get; set; }
}

public class SpecialAttackPrerequisite
{
    public Guid SpecialAttackId { get; set; }
    public int SuccessCount { get; set; }
}
