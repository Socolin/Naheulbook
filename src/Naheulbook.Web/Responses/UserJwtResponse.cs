// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Naheulbook.Web.Responses;

public class UserJwtResponse
{
    public string Token { get; set; } = null!;
    public UserInfoResponse UserInfo { get; set; } = null!;
}