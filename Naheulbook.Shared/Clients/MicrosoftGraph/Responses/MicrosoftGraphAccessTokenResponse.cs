using Newtonsoft.Json;

namespace Naheulbook.Shared.Clients.MicrosoftGraph.Responses;

public class MicrosoftGraphAccessTokenResponse
{
    [JsonProperty("token_type")]
    public string TokenType { get; set; } = null!;

    [JsonProperty("expires_in")]
    public int ExpiresIn { get; set; }

    [JsonProperty("scope")]
    public string Scope { get; set; } = null!;

    [JsonProperty("access_token")]
    public string AccessToken { get; set; } = null!;

    [JsonProperty("refresh_token")]
    public string RefreshToken { get; set; } = null!;
}