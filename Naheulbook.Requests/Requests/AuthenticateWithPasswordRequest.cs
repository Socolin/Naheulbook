// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable UnusedMember.Global

namespace Naheulbook.Requests.Requests
{
    public class AuthenticateWithPasswordRequest
    {
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
}