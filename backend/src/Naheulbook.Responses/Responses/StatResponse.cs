using JetBrains.Annotations;

namespace Naheulbook.Responses.Responses;

[PublicAPI]
public class StatResponse
{
    public string Name { get; set; } = null!;
    public string DisplayName { get; set; } = null!;
    public string Description { get; set; } = null!;
}