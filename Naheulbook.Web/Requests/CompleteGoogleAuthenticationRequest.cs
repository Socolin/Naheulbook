namespace Naheulbook.Web.Requests
{
    public class CompleteGoogleAuthenticationRequest
    {
        public string Code { get; set; }
        public string LoginToken { get; set; }
        public string RedirectUri { get; set; }
    }
}