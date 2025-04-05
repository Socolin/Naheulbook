using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Naheulbook.Core.Features.Users;
using Naheulbook.Shared.Clients.Google;
using Naheulbook.Shared.Utils;
using Naheulbook.Web.Extensions;
using Naheulbook.Web.Requests;
using Naheulbook.Web.Responses;
using Naheulbook.Web.Services;

namespace Naheulbook.Web.Controllers;

[ApiController]
[Route("api/v2/authentications/google")]
public class GoogleAuthenticationController(
    GoogleOptions options,
    IGoogleClient googleClient,
    IJwtService jwtService,
    IMapper mapper,
    IRngUtil rngUtil,
    ISocialMediaUserLinkService socialMediaUserLinkService
) : ControllerBase
{
    private const string GoogleLoginTokenKey = "googleLoginToken";

    [HttpGet("initOAuthAuthentication")]
    public ActionResult<AuthenticationInitResponse> PostInitOauthAuthentication()
    {
        var loginToken = rngUtil.GetRandomHexString(64);

        HttpContext.Session.SetString(GoogleLoginTokenKey, loginToken);

        return new AuthenticationInitResponse
        {
            LoginToken = loginToken,
            AppKey = options.AppId,
        };
    }

    [HttpPost("completeAuthentication")]
    public async Task<ActionResult<UserJwtResponse>> CompleteGoogleAuthenticationAsync(
        [FromBody] CompleteGoogleAuthenticationRequest request
    )
    {
        var loginToken = HttpContext.Session.GetString(GoogleLoginTokenKey);

        if (loginToken != request.LoginToken)
            return BadRequest();

        var accessToken = await googleClient.GetAccessTokenAsync(request.RedirectUri, request.Code);
        var profile = await googleClient.GetUserProfileAsync(accessToken);

        var currentUserId = HttpContext.Session.GetCurrentUserId();
        if (currentUserId.HasValue)
            await socialMediaUserLinkService.AssociateUserToGoogleIdAsync(currentUserId.Value, profile.Id);

        var user = await socialMediaUserLinkService.GetOrCreateUserFromGoogleAsync(profile.DisplayName, profile.Id);

        HttpContext.Session.SetCurrentUserId(user.Id);
        var token = jwtService.GenerateJwtToken(user.Id);

        return new UserJwtResponse
        {
            Token = token,
            UserInfo = mapper.Map<UserInfoResponse>(user),
        };
    }
}