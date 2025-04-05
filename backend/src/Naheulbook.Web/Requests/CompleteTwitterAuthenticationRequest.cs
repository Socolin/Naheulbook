using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;

namespace Naheulbook.Web.Requests;

[PublicAPI]
public class CompleteTwitterAuthenticationRequest
{
    [Required]
    public string OAuthToken { get; set; } = null!;

    [Required]
    public string OauthVerifier { get; set; } = null!;
}