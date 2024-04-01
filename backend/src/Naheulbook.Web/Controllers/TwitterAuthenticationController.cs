using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Naheulbook.Core.Services;
using Naheulbook.Shared.Clients.Twitter;
using Naheulbook.Web.Extensions;
using Naheulbook.Web.Requests;
using Naheulbook.Web.Responses;
using Naheulbook.Web.Services;

namespace Naheulbook.Web.Controllers;

[ApiController]
[Route("api/v2/authentications/twitter")]
public class TwitterAuthenticationController(
    ITwitterClient twitterClient,
    IJwtService jwtService,
    IMapper mapper,
    ISocialMediaUserLinkService socialMediaUserLinkService
) : ControllerBase
{
    private const string TwitterOauthTokenKey = "twitterOauthToken";

    [HttpGet("initOAuthAuthentication")]
    public async Task<ActionResult<TwitterAuthenticationInitResponse>> PostInitOauthAuthenticationAsync()
    {
        var requestToken =  await twitterClient.GetRequestTokenAsync();

        HttpContext.Session.SetString(TwitterOauthTokenKey, requestToken.OAuthToken);

        return new TwitterAuthenticationInitResponse
        {
            LoginToken = requestToken.OAuthToken,
        };
    }

    [HttpPost("completeAuthentication")]
    public async Task<ActionResult<UserJwtResponse>> CompleteTwitterAuthenticationAsync(
        [FromBody] CompleteTwitterAuthenticationRequest request
    )
    {
        var loginToken = HttpContext.Session.GetString(TwitterOauthTokenKey);
        if (loginToken == null)
            return BadRequest();


        var accessToken = await twitterClient.GetAccessTokenAsync(loginToken, request.OAuthToken, request.OauthVerifier);

        var currentUserId = HttpContext.Session.GetCurrentUserId();
        if (currentUserId.HasValue)
            await socialMediaUserLinkService.AssociateUserToTwitterIdAsync(currentUserId.Value, accessToken.UserId);

        var user = await socialMediaUserLinkService.GetOrCreateUserFromTwitterAsync(accessToken.ScreenName, accessToken.UserId);

        HttpContext.Session.SetCurrentUserId(user.Id);
        var token = jwtService.GenerateJwtToken(user.Id);

        return new UserJwtResponse
        {
            Token = token,
            UserInfo = mapper.Map<UserInfoResponse>(user),
        };
    }
}