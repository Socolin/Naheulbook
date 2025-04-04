using System.ComponentModel.DataAnnotations;

namespace Naheulbook.Requests.Requests;

[PublicAPI]
public class ValidateUserRequest
{
    [StringLength(1000, MinimumLength = 1)]
    public required string ActivationCode { get; set; }
}