namespace Naheulbook.Requests.Requests;

[PublicAPI]
public class CreateUserRequest
{
    public required string Username { get; set; }
    public required string Password { get; set; }
}