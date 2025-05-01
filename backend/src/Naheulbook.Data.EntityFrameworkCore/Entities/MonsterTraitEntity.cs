namespace Naheulbook.Data.EntityFrameworkCore.Entities;

[Serializable]
public class MonsterTraitEntity
{
    public int Id { get; set; }
    public string Description { get; set; } = null!;
    public string? Levels { get; set; }
    public string Name { get; set; } = null!;
}