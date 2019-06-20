namespace Naheulbook.Web.Requests
{
    public class CompleteFacebookAuthenticationRequest
    {
        public string Code { get; set; }
        public string LoginToken { get; set; }
        public string RedirectUri { get; set; }
    }
}