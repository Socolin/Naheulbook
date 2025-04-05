using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;

namespace Naheulbook.Web.Requests;

[PublicAPI]
public class CompleteMicrosoftAuthenticationRequest
{
    [Required]
    public string Code { get; set; } = null!;

    [Required]
    public string LoginToken { get; set; } = null!;

    [Required]
    public string RedirectUri { get; set; } = null!;
}