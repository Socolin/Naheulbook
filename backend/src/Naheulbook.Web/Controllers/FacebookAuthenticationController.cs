using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Naheulbook.Core.Features.Users;
using Naheulbook.Shared.Clients.Facebook;
using Naheulbook.Shared.Utils;
using Naheulbook.Web.Extensions;
using Naheulbook.Web.Requests;
using Naheulbook.Web.Responses;
using Naheulbook.Web.Services;

namespace Naheulbook.Web.Controllers;

[ApiController]
[Route("api/v2/authentications/facebook")]
public class FacebookAuthenticationController(
    FacebookConfiguration configuration,
    IFacebookClient facebookClient,
    IJwtService jwtService,
    IMapper mapper,
    IRngUtil rngUtil,
    ISocialMediaUserLinkService socialMediaUserLinkService
) : ControllerBase
{
    private const string FacebookLoginTokenKey = "facebookLoginToken";

    [HttpGet("initOAuthAuthentication")]
    public ActionResult<AuthenticationInitResponse> PostInitOauthAuthentication()
    {
        var loginToken = rngUtil.GetRandomHexString(64);

        HttpContext.Session.SetString(FacebookLoginTokenKey, loginToken);

        return new AuthenticationInitResponse
        {
            LoginToken = loginToken,
            AppKey = configuration.AppId,
        };
    }

    [HttpPost("completeAuthentication")]
    public async Task<ActionResult<UserJwtResponse>> CompleteFacebookAuthenticationAsync(
        [FromBody] CompleteFacebookAuthenticationRequest request
    )
    {
        var loginToken = HttpContext.Session.GetString(FacebookLoginTokenKey);

        if (loginToken != request.LoginToken)
            return BadRequest();

        var accessToken = await facebookClient.GetAccessTokenAsync(request.RedirectUri, request.Code);
        var profile = await facebookClient.GetUserProfileAsync(accessToken);

        var currentUserId = HttpContext.Session.GetCurrentUserId();
        if (currentUserId.HasValue)
            await socialMediaUserLinkService.AssociateUserToFacebookIdAsync(currentUserId.Value, profile.Id);

        var user = await socialMediaUserLinkService.GetOrCreateUserFromFacebookAsync(profile.Name, profile.Id);

        HttpContext.Session.SetCurrentUserId(user.Id);
        var token = jwtService.GenerateJwtToken(user.Id);

        return new UserJwtResponse
        {
            Token = token,
            UserInfo = mapper.Map<UserInfoResponse>(user),
        };
    }
}