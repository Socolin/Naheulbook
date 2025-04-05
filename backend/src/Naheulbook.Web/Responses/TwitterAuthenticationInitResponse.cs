using JetBrains.Annotations;

namespace Naheulbook.Web.Responses;

[PublicAPI]
public class TwitterAuthenticationInitResponse
{
    public string LoginToken { get; set; } = null!;
}