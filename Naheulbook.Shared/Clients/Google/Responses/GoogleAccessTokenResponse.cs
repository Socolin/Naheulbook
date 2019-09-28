using Newtonsoft.Json;

namespace Naheulbook.Shared.Clients.Google.Responses
{
    public class GoogleAccessTokenResponse
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; } = null!;
    }
}