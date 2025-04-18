using JetBrains.Annotations;

namespace Naheulbook.Web.Responses;

[PublicAPI]
public class EventResponse
{
    public int Id { get; set; }
    public long Timestamp { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
}