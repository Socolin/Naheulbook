namespace Naheulbook.Data.Models
{
    public class MapMarker
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public string Type { get; set; } = null!;
        public string MarkerInfo { get; set; } = null!;

        public int LayerId { get; set; }
        public MapLayer Layer { get; set; } = null!;
    }
}