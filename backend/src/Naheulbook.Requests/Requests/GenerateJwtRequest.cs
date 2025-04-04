using System.ComponentModel.DataAnnotations;

namespace Naheulbook.Requests.Requests;

[PublicAPI]
public class GenerateJwtRequest
{
    [StringLength(256, MinimumLength = 64)]
    public required string Password { get; set; }
}