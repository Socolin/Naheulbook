namespace Naheulbook.Requests.Requests;

[PublicAPI]
public class GenerateJwtRequest
{
    public required string Password { get; set; }
}