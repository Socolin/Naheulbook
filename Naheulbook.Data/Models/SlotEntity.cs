namespace Naheulbook.Data.Models;

public class SlotEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string TechName { get; set; } = null!;
    public short Count { get; set; }
    public bool? Stackable { get; set; }
}