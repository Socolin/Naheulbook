using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Naheulbook.Shared.Clients.Facebook.Exceptions;
using Naheulbook.Shared.Clients.Facebook.Responses;
using Naheulbook.Shared.Utils;

namespace Naheulbook.Shared.Clients.Facebook;

public interface IFacebookClient
{
    Task<string> GetAccessTokenAsync(string redirectUri, string code);
    Task<FacebookProfileResponse> GetUserProfileAsync(string accessToken);
}

public class FacebookClient(IOptions<FacebookOptions> options, IJsonUtil jsonUtil) : IFacebookClient
{
    public async Task<string> GetAccessTokenAsync(string redirectUri, string code)
    {
        var requestUri = new StringBuilder();
        requestUri.AppendFormat("https://graph.facebook.com/v21.0/oauth/access_token");
        requestUri.Append($"?redirect_uri={Uri.EscapeDataString(redirectUri)}");
        requestUri.Append($"&code={Uri.EscapeDataString(code)}");
        requestUri.Append($"&client_id={options.Value.AppId}");
        requestUri.Append($"&client_secret={options.Value.AppSecret}");

        using var client = new HttpClient();
        using var response = await client.GetAsync(requestUri.ToString());

        var content = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
            throw new FacebookClientException(content, (int)response.StatusCode);

        return jsonUtil.DeserializeOrCreate<FacebookAccessTokenResponse>(content).AccessToken;
    }

    public async Task<FacebookProfileResponse> GetUserProfileAsync(string accessToken)
    {
        var requestUri = $"https://graph.facebook.com/me?access_token={Uri.EscapeDataString(accessToken)}";

        using var client = new HttpClient();
        using var response = await client.GetAsync(requestUri);

        var content = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
            throw new FacebookClientException(content, (int)response.StatusCode);

        return jsonUtil.DeserializeOrCreate<FacebookProfileResponse>(content);
    }
}