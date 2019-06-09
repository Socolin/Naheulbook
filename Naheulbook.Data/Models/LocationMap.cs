namespace Naheulbook.Data.Models
{
    public class LocationMap
    {
        public int Id { get; set; }
        public string Data { get; set; }
        public string File { get; set; }
        public bool IsGm { get; set; }
        public string Name { get; set; }

        public int LocationId { get; set; }
        public Location Location { get; set; }
    }
}