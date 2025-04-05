using JetBrains.Annotations;

namespace Naheulbook.Web.Responses;

[PublicAPI]
public class AuthenticationInitResponse
{
    public string LoginToken { get; set; } = null!;
    public string AppKey { get; set; } = null!;
}