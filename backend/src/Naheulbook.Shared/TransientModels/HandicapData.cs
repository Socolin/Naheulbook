namespace Naheulbook.Shared.TransientModels;

public class HandicapData
{
    /// <summary>
    /// What the character must have for this handicap to apply (origin, skill...). Null when it applies to everyone.
    /// </summary>
    public string? Condition { get; set; }

    public string Effect { get; set; } = null!;
}
