namespace Naheulbook.Data.EntityFrameworkCore.Entities;

public class MankdebolCalamityEntity : IEntropyEventEntity
{
    public Guid Id { get; set; }

    /// <summary>
    /// Result of the D100 roll that gives this calamity
    /// </summary>
    public int DiceRoll { get; set; }

    public string Name { get; set; } = null!;
    public string Effect { get; set; } = null!;
}
