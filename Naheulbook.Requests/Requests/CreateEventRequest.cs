// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

namespace Naheulbook.Requests.Requests
{
    public class CreateEventRequest
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public long Timestamp { get; set; }
    }
}