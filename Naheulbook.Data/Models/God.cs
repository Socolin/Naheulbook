namespace Naheulbook.Data.Models
{
    public class God
    {
        public int Id { get; set; }
        public string DisplayName { get; set; } = null!;
        public string? Description { get; set; }
        public string TechName { get; set; } = null!;
    }
}