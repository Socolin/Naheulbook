using JetBrains.Annotations;

namespace Naheulbook.Responses.Responses;

[PublicAPI]
public class TwitterAuthenticationInitResponse
{
    public string LoginToken { get; set; } = null!;
}