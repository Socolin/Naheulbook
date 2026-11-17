using JetBrains.Annotations;

namespace Naheulbook.Responses.Responses;

[PublicAPI]
public class UserJwtResponse
{
    public string Token { get; set; } = null!;
    public UserInfoResponse UserInfo { get; set; } = null!;
}