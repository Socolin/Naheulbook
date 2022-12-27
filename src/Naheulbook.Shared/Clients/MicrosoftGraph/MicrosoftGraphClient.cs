using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Naheulbook.Shared.Clients.MicrosoftGraph.Exceptions;
using Naheulbook.Shared.Clients.MicrosoftGraph.Responses;
using Naheulbook.Shared.Utils;

namespace Naheulbook.Shared.Clients.MicrosoftGraph;

public interface IMicrosoftGraphClient
{
    Task<MicrosoftGraphAccessTokenResponse> GetAccessTokenAsync(string redirectUri, string code);
    Task<MicrosoftGraphProfileResponse> GetUserProfileAsync(MicrosoftGraphAccessTokenResponse accessToken);
}

public class MicrosoftGraphClient : IMicrosoftGraphClient
{
    private const string TokenApiRequestUri = "https://login.microsoftonline.com/common/oauth2/v2.0/token";
    private const string AccessApiRequestUri = "https://graph.microsoft.com/v1.0/me?$select=id,displayName";

    private readonly MicrosoftGraphConfiguration _configuration;
    private readonly IJsonUtil _jsonUtil;

    public MicrosoftGraphClient(MicrosoftGraphConfiguration configuration, IJsonUtil jsonUtil)
    {
        _configuration = configuration;
        _jsonUtil = jsonUtil;
    }

    public async Task<MicrosoftGraphAccessTokenResponse> GetAccessTokenAsync(string redirectUri, string code)
    {
        var requestArgs = new Dictionary<string, string>
        {
            ["redirect_uri"] = redirectUri,
            ["code"] = code,
            ["grant_type"] = "authorization_code",
            ["client_id"] = _configuration.AppId,
            ["client_secret"] = _configuration.AppSecret,
        };

        using (var client = new HttpClient())
        {
            using (var response = await client.PostAsync(TokenApiRequestUri, new FormUrlEncodedContent(requestArgs)))
            {
                var content = await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode)
                    throw new MicrosoftGraphClientException(content, (int) response.StatusCode);

                return _jsonUtil.DeserializeOrCreate<MicrosoftGraphAccessTokenResponse>(content);
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
                    throw new MicrosoftGraphClientException(content, (int) response.StatusCode);

                return _jsonUtil.DeserializeOrCreate<MicrosoftGraphProfileResponse>(content);
            }
        }
    }
}