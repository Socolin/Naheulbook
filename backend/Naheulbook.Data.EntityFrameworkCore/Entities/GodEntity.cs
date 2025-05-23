namespace Naheulbook.Data.EntityFrameworkCore.Entities;

[Serializable]
public class GodEntity
{
    public int Id { get; set; }
    public string DisplayName { get; set; } = null!;
    public string? Description { get; set; }
    public string TechName { get; set; } = null!;
}