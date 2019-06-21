using Newtonsoft.Json;

namespace Naheulbook.Shared.Clients.Twitter.Responses
{
    public class TwitterAccessTokenResponse
    {
        [JsonProperty("user_id")]
        public string UserId { get; set; }

        [JsonProperty("screen_name")]
        public string ScreenName { get; set; }
    }
}