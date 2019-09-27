namespace Naheulbook.Data.Models
{
    public class LocationMap
    {
        public int Id { get; set; }
        public string Data { get; set; } = null!;
        public string File { get; set; } = null!;
        public bool IsGm { get; set; }
        public string Name { get; set; } = null!;

        public int LocationId { get; set; }
        public Location Location { get; set; } = null!;
    }
}