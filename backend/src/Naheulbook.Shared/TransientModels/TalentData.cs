namespace Naheulbook.Shared.TransientModels;

public class TalentData
{
    /// <summary>
    /// Who can take the talent (origin, job, skill...), as written in the book. Null when the talent is open to everyone.
    /// </summary>
    public string? Restriction { get; set; }

    public string Effect { get; set; } = null!;
    public string? Notes { get; set; }
    public int? RequiredLevel { get; set; }

    /// <summary>
    /// Talent that must be known before this one can be taken (e.g. the base mastery of a "+1" mastery).
    /// </summary>
    public Guid? PrerequisiteTalentId { get; set; }
}
