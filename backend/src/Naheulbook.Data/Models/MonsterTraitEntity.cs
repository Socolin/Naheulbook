namespace Naheulbook.Data.Models;

public class MonsterTraitEntity
{
    public int Id { get; set; }
    public string Description { get; set; } = null!;
    public string? Levels { get; set; }
    public string Name { get; set; } = null!;
}