using JetBrains.Annotations;

namespace Naheulbook.Responses.Responses;

[PublicAPI]
public class ItemTypeResponse
{
    public int Id { get; set; }
    public string DisplayName { get; set; } = null!;
    public string TechName { get; set; } = null!;
}