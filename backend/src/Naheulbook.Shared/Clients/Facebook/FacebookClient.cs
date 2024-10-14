using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Naheulbook.Shared.Clients.Facebook.Exceptions;
using Naheulbook.Shared.Clients.Facebook.Responses;
using Naheulbook.Shared.Utils;

namespace Naheulbook.Shared.Clients.Facebook;

public interface IFacebookClient
{
    Task<string> GetAccessTokenAsync(string redirectUri, string code);
    Task<FacebookProfileResponse> GetUserProfileAsync(string accessToken);
}

public class FacebookClient(FacebookConfiguration configuration, IJsonUtil jsonUtil) : IFacebookClient
{
    public async Task<string> GetAccessTokenAsync(string redirectUri, string code)
    {
        var requestUri = new StringBuilder();
        requestUri.AppendFormat("https://graph.facebook.com/v21.0/oauth/access_token");
        requestUri.AppendFormat("?redirect_uri={0}", Uri.EscapeDataString(redirectUri));
        requestUri.AppendFormat("&code={0}", Uri.EscapeDataString(code));
        requestUri.AppendFormat("&client_id={0}", configuration.AppId);
        requestUri.AppendFormat("&client_secret={0}", configuration.AppSecret);

        using (var client = new HttpClient())
        {
            using (var response = await client.GetAsync(requestUri.ToString()))
            {
                var content = await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode)
                    throw new FacebookClientException(content, (int) response.StatusCode);

                return jsonUtil.DeserializeOrCreate<FacebookAccessTokenResponse>(content).AccessToken;
            }
        }
    }

    public async Task<FacebookProfileResponse> GetUserProfileAsync(string accessToken)
    {
        var requestUri = $"https://graph.facebook.com/me?access_token={Uri.EscapeDataString(accessToken)}";

        using (var client = new HttpClient())
        {
            using (var response = await client.GetAsync(requestUri))
            {
                var content = await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode)
                    throw new FacebookClientException(content, (int) response.StatusCode);

                return jsonUtil.DeserializeOrCreate<FacebookProfileResponse>(content);
            }
        }
    }
}