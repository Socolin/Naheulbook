using System;
using System.Threading.Tasks;
using Naheulbook.Shared.Clients.Oauth1;
using Naheulbook.Shared.Clients.Twitter.Exceptions;
using Naheulbook.Shared.Clients.Twitter.Responses;

namespace Naheulbook.Shared.Clients.Twitter;

public interface ITwitterClient
{
    Task<TwitterRequestTokenResponse> GetRequestTokenAsync();
    Task<TwitterAccessTokenResponse> GetAccessTokenAsync(string loginToken, string oauthToken, string oauthVerifier);
}

public class TwitterClient(TwitterConfiguration configuration) : ITwitterClient
{
    private const string RequestTokenUri = "https://api.twitter.com/oauth/request_token";
    private const string AccessTokenUri = "https://api.twitter.com/oauth/access_token";

    public async Task<TwitterRequestTokenResponse> GetRequestTokenAsync()
    {
        var oauth = new Oauth(configuration.AppId, configuration.AppSecret, RequestTokenUri);
        oauth.AddOauthParameter("callback", configuration.Callback);

        try
        {
            var oauthResult = await oauth.DoRequest();

            return new TwitterRequestTokenResponse
            {
                OAuthToken = oauthResult["oauth_token"],
            };
        }
        catch (Exception ex)
        {
            throw new TwitterClientException(ex);
        }
    }

    public async Task<TwitterAccessTokenResponse> GetAccessTokenAsync(string loginToken, string oauthToken, string oauthVerifier)
    {
        var oauth = new Oauth(configuration.AppId, configuration.AppSecret, AccessTokenUri)
        {
            AccessSecret = loginToken,
        };
        oauth.AddOauthParameter("token", oauthToken);
        oauth.AddParameter("oauth_verifier", oauthVerifier);

        try
        {
            var oauthResult = await oauth.DoRequest();

            return new TwitterAccessTokenResponse
            {
                ScreenName = oauthResult["screen_name"],
                UserId = oauthResult["user_id"],
            };
        }
        catch (Exception ex)
        {
            throw new TwitterClientException(ex);
        }
    }
}