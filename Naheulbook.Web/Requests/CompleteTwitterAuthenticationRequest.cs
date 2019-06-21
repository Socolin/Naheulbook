namespace Naheulbook.Web.Requests
{
    public class CompleteTwitterAuthenticationRequest
    {
        public string OAuthToken { get; set; }
        public string OauthVerifier { get; set; }
    }
}