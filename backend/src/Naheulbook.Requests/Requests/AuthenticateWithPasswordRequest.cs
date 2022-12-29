namespace Naheulbook.Requests.Requests;

[PublicAPI]
public class AuthenticateWithPasswordRequest
{
    public required string Username { get; set; }
    public required string Password { get; set; }
}