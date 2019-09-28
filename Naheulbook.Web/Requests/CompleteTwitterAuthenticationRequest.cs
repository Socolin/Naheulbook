using System.ComponentModel.DataAnnotations;

// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

namespace Naheulbook.Web.Requests
{
    public class CompleteTwitterAuthenticationRequest
    {
        [Required]
        public string OAuthToken { get; set; } = null!;

        [Required]
        public string OauthVerifier { get; set; } = null!;
    }
}