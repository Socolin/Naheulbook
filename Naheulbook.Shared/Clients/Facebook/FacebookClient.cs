using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Naheulbook.Shared.Clients.Facebook.Exceptions;
using Naheulbook.Shared.Clients.Facebook.Responses;
using Naheulbook.Shared.Utils;

namespace Naheulbook.Shared.Clients.Facebook
{
    public interface IFacebookClient
    {
        Task<string> GetAccessTokenAsync(string redirectUri, string code);
        Task<FacebookProfileResponse> GetUserProfileAsync(string accessToken);
    }

    public class FacebookClient : IFacebookClient
    {
        private readonly FacebookConfiguration _configuration;
        private readonly IJsonUtil _jsonUtil;

        public FacebookClient(FacebookConfiguration configuration, IJsonUtil jsonUtil)
        {
            _configuration = configuration;
            _jsonUtil = jsonUtil;
        }

        public async Task<string> GetAccessTokenAsync(string redirectUri, string code)
        {
            var requestUri = new StringBuilder();
            requestUri.AppendFormat("https://graph.facebook.com/v6.0/oauth/access_token");
            requestUri.AppendFormat("?redirect_uri={0}", Uri.EscapeUriString(redirectUri));
            requestUri.AppendFormat("&code={0}", Uri.EscapeUriString(code));
            requestUri.AppendFormat("&client_id={0}", _configuration.AppId);
            requestUri.AppendFormat("&client_secret={0}", _configuration.AppSecret);

            using (var client = new HttpClient())
            {
                using (var response = await client.GetAsync(requestUri.ToString()))
                {
                    var content = await response.Content.ReadAsStringAsync();
                    if (!response.IsSuccessStatusCode)
                        throw new FacebookClientException(content, (int) response.StatusCode);

                    return _jsonUtil.DeserializeOrCreate<FacebookAccessTokenResponse>(content).AccessToken;
                }
            }
        }

        public async Task<FacebookProfileResponse> GetUserProfileAsync(string accessToken)
        {
            var requestUri = $"https://graph.facebook.com/me?access_token={Uri.EscapeUriString(accessToken)}";

            using (var client = new HttpClient())
            {
                using (var response = await client.GetAsync(requestUri))
                {
                    var content = await response.Content.ReadAsStringAsync();
                    if (!response.IsSuccessStatusCode)
                        throw new FacebookClientException(content, (int) response.StatusCode);

                    return _jsonUtil.DeserializeOrCreate<FacebookProfileResponse>(content);
                }
            }
        }
    }
}