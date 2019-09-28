// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Naheulbook.Web.Responses
{
    public class AuthenticationInitResponse
    {
        public string LoginToken { get; set; } = null!;
        public string AppKey { get; set; } = null!;
    }
}