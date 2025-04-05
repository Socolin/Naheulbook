using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Naheulbook.Shared.Clients.MicrosoftGraph.Exceptions;
using Naheulbook.Shared.Clients.MicrosoftGraph.Responses;
using Naheulbook.Shared.Utils;

namespace Naheulbook.Shared.Clients.MicrosoftGraph;

public interface IMicrosoftGraphClient
{
    Task<MicrosoftGraphAccessTokenResponse> GetAccessTokenAsync(string redirectUri, string code);
    Task<MicrosoftGraphProfileResponse> GetUserProfileAsync(MicrosoftGraphAccessTokenResponse accessToken);
}

public class MicrosoftGraphClient(IOptions<MicrosoftGraphOptions> configuration, IJsonUtil jsonUtil)
    : IMicrosoftGraphClient
{
    private const string TokenApiRequestUri = "https://login.microsoftonline.com/common/oauth2/v2.0/token";
    private const string AccessApiRequestUri = "https://graph.microsoft.com/v1.0/me?$select=id,displayName";

    public async Task<MicrosoftGraphAccessTokenResponse> GetAccessTokenAsync(string redirectUri, string code)
    {
        var requestArgs = new Dictionary<string, string>
        {
            ["redirect_uri"] = redirectUri,
            ["code"] = code,
            ["grant_type"] = "authorization_code",
            ["client_id"] = configuration.Value.AppId,
            ["client_secret"] = configuration.Value.AppSecret,
        };

        using (var client = new HttpClient())
        {
            using (var response = await client.PostAsync(TokenApiRequestUri, new FormUrlEncodedContent(requestArgs)))
            {
                var content = await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode)
                    throw new MicrosoftGraphClientException(content, (int)response.StatusCode);

                return jsonUtil.DeserializeOrCreate<MicrosoftGraphAccessTokenResponse>(content);
            }
        }
    }

    public async Task<MicrosoftGraphProfileResponse> GetUserProfileAsync(MicrosoftGraphAccessTokenResponse token)
    {
        using (var client = new HttpClient())
        {
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, AccessApiRequestUri);
            httpRequestMessage.Headers.Authorization = new AuthenticationHeaderValue(token.TokenType, token.AccessToken);
            using (var response = await client.SendAsync(httpRequestMessage))
            {
                var content = await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode)
                    throw new MicrosoftGraphClientException(content, (int)response.StatusCode);

                return jsonUtil.DeserializeOrCreate<MicrosoftGraphProfileResponse>(content);
            }
        }
    }
}