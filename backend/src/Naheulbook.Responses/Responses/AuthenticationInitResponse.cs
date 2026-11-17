using JetBrains.Annotations;

namespace Naheulbook.Responses.Responses;

[PublicAPI]
public class AuthenticationInitResponse
{
    public string LoginToken { get; set; } = null!;
    public string AppKey { get; set; } = null!;
}