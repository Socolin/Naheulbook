namespace Naheulbook.Data.EntityFrameworkCore.Entities;

[Serializable]
public class ItemTypeEntity
{
    public int Id { get; set; }
    public string DisplayName { get; set; } = null!;
    public string TechName { get; set; } = null!;
}