using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Naheulbook.Core.Features.Users;
using Naheulbook.Shared.Clients.MicrosoftGraph;
using Naheulbook.Shared.Utils;
using Naheulbook.Web.Extensions;
using Naheulbook.Web.Requests;
using Naheulbook.Web.Responses;
using Naheulbook.Web.Services;

namespace Naheulbook.Web.Controllers;

[ApiController]
[Route("api/v2/authentications/microsoft")]
public class MicrosoftAuthenticationController(
    MicrosoftGraphConfiguration configuration,
    IMicrosoftGraphClient microsoftGraphClient,
    IJwtService jwtService,
    IMapper mapper,
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
            AppKey = configuration.AppId,
        };
    }

    [HttpPost("completeAuthentication")]
    public async Task<ActionResult<UserJwtResponse>> CompleteMicrosoftAuthenticationAsync(
        [FromBody] CompleteMicrosoftAuthenticationRequest request
    )
    {
        var loginToken = HttpContext.Session.GetString(MicrosoftLoginTokenKey);

        if (loginToken != request.LoginToken)
            return BadRequest();

        var accessToken = await microsoftGraphClient.GetAccessTokenAsync(request.RedirectUri, request.Code);
        var profile = await microsoftGraphClient.GetUserProfileAsync(accessToken);

        var currentUserId = HttpContext.Session.GetCurrentUserId();
        if (currentUserId.HasValue)
            await socialMediaUserLinkService.AssociateUserToMicrosoftIdAsync(currentUserId.Value, profile.Id);

        var user = await socialMediaUserLinkService.GetOrCreateUserFromMicrosoftAsync(profile.Name, profile.Id);

        HttpContext.Session.SetCurrentUserId(user.Id);
        var token = jwtService.GenerateJwtToken(user.Id);

        return new UserJwtResponse
        {
            Token = token,
            UserInfo = mapper.Map<UserInfoResponse>(user),
        };
    }
}