using System.ComponentModel.DataAnnotations;

namespace Naheulbook.Requests.Requests;

[PublicAPI]
public class CreateUserRequest
{
    public required string Username { get; set; }

    [StringLength(256, MinimumLength = 64)]
    public required string Password { get; set; }
}