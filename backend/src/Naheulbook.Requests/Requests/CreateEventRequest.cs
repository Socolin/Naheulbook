namespace Naheulbook.Requests.Requests;

[PublicAPI]
public class CreateEventRequest
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public long Timestamp { get; set; }
}