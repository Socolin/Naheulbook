namespace Naheulbook.Data.EntityFrameworkCore.Entities;

public class HandicapEntity
{
    public Guid Id { get; set; }

    /// <summary>
    /// Result of the D100 roll that gives this handicap
    /// </summary>
    public int DiceRoll { get; set; }

    public string Name { get; set; } = null!;
    public string Data { get; set; } = null!;
}
