namespace Naheulbook.Data.EntityFrameworkCore.Entities;

/// <summary>
/// An entry of one of the D100 entropy tables (Mankdebol calamities or Débilibeuk blessings)
/// </summary>
public interface IEntropyEventEntity
{
    Guid Id { get; }
    int DiceRoll { get; }
    string Name { get; }
    string Effect { get; }
}
