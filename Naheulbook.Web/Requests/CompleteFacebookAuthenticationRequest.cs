using System.ComponentModel.DataAnnotations;

// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

namespace Naheulbook.Web.Requests;

public class CompleteFacebookAuthenticationRequest
{
    [Required]
    public string Code { get; set; } = null!;

    [Required]
    public string LoginToken { get; set; } = null!;

    [Required]
    public string RedirectUri { get; set; } = null!;
}