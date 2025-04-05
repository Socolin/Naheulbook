using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Naheulbook.Shared.Clients.Google.Exceptions;
using Naheulbook.Shared.Clients.Google.Responses;
using Naheulbook.Shared.Utils;

namespace Naheulbook.Shared.Clients.Google;

public interface IGoogleClient
{
    Task<string> GetAccessTokenAsync(string redirectUri, string code);
    Task<GoogleProfileResponse> GetUserProfileAsync(string accessToken);
}

public class GoogleClient(IOptions<GoogleOptions> options, IJsonUtil jsonUtil) : IGoogleClient
{
    private const string TokenApiRequestUri = "https://www.googleapis.com/oauth2/v4/token";
    private const string AccessApiRequestUri = "https://www.googleapis.com/plus/v1/people/me";

    public async Task<string> GetAccessTokenAsync(string redirectUri, string code)
    {
        var requestArgs = new Dictionary<string, string>
        {
            ["redirect_uri"] = redirectUri,
            ["code"] = code,
            ["grant_type"] = "authorization_code",
            ["client_id"] = options.Value.AppId,
            ["client_secret"] = options.Value.AppSecret,
        };

        using var client = new HttpClient();

        using var response = await client.PostAsync(TokenApiRequestUri, new FormUrlEncodedContent(requestArgs));
        var content = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
            throw new GoogleClientException(content, (int)response.StatusCode);

        return jsonUtil.DeserializeOrCreate<GoogleAccessTokenResponse>(content).AccessToken;
    }

    public async Task<GoogleProfileResponse> GetUserProfileAsync(string accessToken)
    {
        using var client = new HttpClient();
        var profileRequestUri = $"{AccessApiRequestUri}?access_token={Uri.EscapeDataString(accessToken)}";

        using var response = await client.GetAsync(profileRequestUri);
        var content = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
            throw new GoogleClientException(content, (int)response.StatusCode);

        return jsonUtil.DeserializeOrCreate<GoogleProfileResponse>(content);
    }
}