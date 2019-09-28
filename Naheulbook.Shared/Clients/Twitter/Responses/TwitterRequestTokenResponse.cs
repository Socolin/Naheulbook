using Newtonsoft.Json;

namespace Naheulbook.Shared.Clients.Twitter.Responses
{
    public class TwitterRequestTokenResponse
    {
        [JsonProperty("oauth_token")]
        public string OAuthToken { get; set; } = null!;
    }
}