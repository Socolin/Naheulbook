using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Naheulbook.Core.Features.Users;
using Naheulbook.Responses.Responses;
using Naheulbook.Shared.Clients.MicrosoftGraph;
using Naheulbook.Shared.Utils;
using Naheulbook.Web.Extensions;
using Naheulbook.Web.Requests;
using Naheulbook.Web.Services;

namespace Naheulbook.Web.Controllers;

[ApiController]
[Route("api/v2/authentications/microsoft")]
public class MicrosoftAuthenticationController(
    IOptions<MicrosoftGraphOptions> configuration,
    IMicrosoftGraphClient microsoftGraphClient,
    IJwtService jwtService,
    IUserMapper userMapper,
    IRngUtil rngUtil,
    ISocialMediaUserLinkService socialMediaUserLinkService
) : ControllerBase
{
    private const string MicrosoftLoginTokenKey = "microsoftLoginToken";

    [HttpGet("initOAuthAuthentication")]
    public ActionResult<AuthenticationInitResponse> PostInitOauthAuthentication()
    {
        var loginToken = rngUtil.GetRandomHexString(64);

        HttpContext.Session.SetString(MicrosoftLoginTokenKey, loginToken);

        return new AuthenticationInitResponse
        {
            LoginToken = loginToken,
            AppKey = configuration.Value.AppId,
        };
    }

    [HttpPost("completeAuthentication")]
    public async Task<ActionResult<UserJwtResponse>> CompleteMicrosoftAuthenticationAsync(
        [FromBody] CompleteMicrosoftAuthenticationRequest request,
        CancellationToken cancellationToken
    )
    {
        var loginToken = HttpContext.Session.GetString(MicrosoftLoginTokenKey);

        if (loginToken != request.LoginToken)
            return BadRequest();

        var accessToken = await microsoftGraphClient.GetAccessTokenAsync(request.RedirectUri, request.Code);
        var profile = await microsoftGraphClient.GetUserProfileAsync(accessToken);

        var currentUserId = HttpContext.Session.GetCurrentUserId();
        if (currentUserId.HasValue)
            await socialMediaUserLinkService.AssociateUserToMicrosoftIdAsync(currentUserId.Value, profile.Id, cancellationToken);

        var user = await socialMediaUserLinkService.GetOrCreateUserFromMicrosoftAsync(profile.Name, profile.Id, cancellationToken);

        HttpContext.Session.SetCurrentUserId(user.Id);
        var token = jwtService.GenerateJwtToken(user.Id);

        return new UserJwtResponse
        {
            Token = token,
            UserInfo = userMapper.ToInfoResponse(user),
        };
    }
}