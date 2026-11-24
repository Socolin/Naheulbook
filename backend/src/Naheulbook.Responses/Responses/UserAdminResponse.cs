using JetBrains.Annotations;

namespace Naheulbook.Responses.Responses;

[PublicAPI]
public class UserAdminResponse
{
    public int Id { get; set; }
    public string? Username { get; set; }
    public string? DisplayName { get; set; }
    public int AdminLevel { get; set; }
    public string? FacebookId { get; set; }
    public string? GoogleId { get; set; }
    public string? MicrosoftId { get; set; }
    public string? TwitterId { get; set; }
}
