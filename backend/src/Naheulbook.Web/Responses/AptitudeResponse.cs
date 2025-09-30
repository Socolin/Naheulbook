using JetBrains.Annotations;

namespace Naheulbook.Web.Responses;

[PublicAPI]
public class AptitudeResponse
{
    public Guid Id { get; set; }
    public int Roll { get; set; }
    public string Type { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string Effect { get; set; } = null!;
}