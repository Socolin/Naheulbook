namespace Naheulbook.Requests.Requests
{
    public class CreateEventRequest
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public long Timestamp { get; set; }
    }
}