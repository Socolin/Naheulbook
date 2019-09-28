using Newtonsoft.Json;

namespace Naheulbook.Shared.Clients.Facebook.Responses
{
    public class FacebookAccessTokenResponse
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; } = null!;
    }
}