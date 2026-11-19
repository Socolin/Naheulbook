namespace Naheulbook.Shared.TransientModels;

public class MasteryData
{
    /// <summary>
    /// Who can take the mastery (e.g. armor natural PR range), as written in the book. Null when there is no restriction.
    /// </summary>
    public string? Restriction { get; set; }

    public string Effect { get; set; } = null!;
    public string? Notes { get; set; }
    public int? RequiredLevel { get; set; }

    /// <summary>
    /// Mastery that must be known before this one can be taken (the base mastery of a "+1" mastery).
    /// </summary>
    public Guid? PrerequisiteMasteryId { get; set; }
}
