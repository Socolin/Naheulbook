namespace Naheulbook.Web.Responses
{
    public class EventResponse
    {
        public int Id { get; set; }
        public long Timestamp { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
    }
}