using Microsoft.AspNetCore.Mvc;
using Naheulbook.Core.Features.Shared;
using Naheulbook.Core.Features.Users;
using Naheulbook.Requests.Requests;
using Naheulbook.Responses.Responses;
using Naheulbook.Web.ActionResults;
using Naheulbook.Web.Exceptions;
using Naheulbook.Web.Extensions;
using Naheulbook.Web.Services;

namespace Naheulbook.Web.Controllers;

[Route("api/v2/users")]
[ApiController]
public class UsersController(IUserService userService, IJwtService jwtService, IUserMapper userMapper, IUserAccessTokenService userAccessTokenService)
    : ControllerBase
{
    [HttpPost]
    public async Task<StatusCodeResult> PostAsync(CreateUserRequest request, CancellationToken cancellationToken)
    {
        try
        {
            await userService.CreateUserAsync(request.Username, request.Password, cancellationToken);
        }
        catch (UserAlreadyExistsException)
        {
            return Conflict();
        }

        return StatusCode(StatusCodes.Status201Created);
    }

    [HttpPost("{Username}/validate")]
    public async Task<StatusCodeResult> PostValidateUserAsync(string username, ValidateUserRequest request, CancellationToken cancellationToken)
    {
        try
        {
            await userService.ValidateUserAsync(username, request.ActivationCode, cancellationToken);
        }
        catch (UserNotFoundException)
        {
            return StatusCode(StatusCodes.Status403Forbidden);
        }
        catch (InvalidUserActivationCodeException)
        {
            return StatusCode(StatusCodes.Status403Forbidden);
        }

        return StatusCode(StatusCodes.Status204NoContent);
    }

    [HttpPost("{Username}/jwt")]
    public async Task<ActionResult<UserJwtResponse>> PostGenerateJwtAsync(string username, GenerateJwtRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var user = await userService.CheckPasswordAsync(username, request.Password, cancellationToken);
            HttpContext.Session.SetCurrentUserId(user.Id);
            var token = jwtService.GenerateJwtToken(user.Id);
            return new UserJwtResponse {Token = token};
        }
        catch (UserNotFoundException)
        {
            return StatusCode(StatusCodes.Status401Unauthorized);
        }
        catch (InvalidPasswordException)
        {
            return StatusCode(StatusCodes.Status401Unauthorized);
        }
    }

    [HttpGet("me")]
    public async Task<ActionResult<UserInfoResponse>> GetCurrentUserInfoAsync(CancellationToken cancellationToken)
    {
        // FIXME: userSession if userId is not found, check in db if long duration session still valid
        var userId = HttpContext.Session.GetCurrentUserId();
        if (!userId.HasValue)
            return StatusCode(StatusCodes.Status401Unauthorized);

        var user = await userService.GetUserInfoAsync(userId.Value, cancellationToken);
        return userMapper.ToInfoResponse(user);
    }

    [HttpGet("me/jwt")]
    public async Task<ActionResult<UserJwtResponse>> GetAJwtAsync(CancellationToken cancellationToken)
    {
        // FIXME: userSession if userId is not found, check in db if long duration session still valid
        var userId = HttpContext.Session.GetCurrentUserId();
        if (!userId.HasValue)
            return StatusCode(StatusCodes.Status401Unauthorized);

        var userInfo = await userService.GetUserInfoAsync(userId.Value, cancellationToken);
        var token = jwtService.GenerateJwtToken(userId.Value);

        return new UserJwtResponse
        {
            Token = token,
            UserInfo = userMapper.ToInfoResponse(userInfo),
        };
    }

    [HttpGet("me/accessTokens")]
    public async Task<ActionResult<List<UserAccessTokenResponse>>> GetUserAccessTokens(CancellationToken cancellationToken)
    {
        // FIXME: userSession if userId is not found, check in db if long duration session still valid
        var userId = HttpContext.Session.GetCurrentUserId();
        if (!userId.HasValue)
            return StatusCode(StatusCodes.Status401Unauthorized);

        var accessTokens = await userAccessTokenService.GetUserAccessTokensAsync(userId.Value, cancellationToken);
        return accessTokens.Select(userMapper.ToResponse).ToList();
    }

    [HttpPost("me/accessTokens")]
    public async Task<CreatedActionResult<UserAccessTokenResponseWithKey>> PostCreateUserAccessToken(
        [FromBody] CreateAccessTokenRequest request,
        CancellationToken cancellationToken
    )
    {
        // FIXME: userSession if userId is not found, check in db if long duration session still valid
        var userId = HttpContext.Session.GetCurrentUserId();
        if (!userId.HasValue)
            return StatusCode(StatusCodes.Status401Unauthorized);

        var accessToken= await userAccessTokenService.CreateUserAccessTokenAsync(userId.Value, request, cancellationToken);

        return userMapper.ToResponseWithKey(accessToken);
    }

    [HttpDelete("me/accessTokens/{UserAccessTokenId}")]
    public async Task<ActionResult<List<UserAccessTokenResponse>>> DeleteUserAccessToken(
        [FromRoute] Guid userAccessTokenId,
        CancellationToken cancellationToken
    )
    {
        // FIXME: userSession if userId is not found, check in db if long duration session still valid
        var userId = HttpContext.Session.GetCurrentUserId();
        if (!userId.HasValue)
            return StatusCode(StatusCodes.Status401Unauthorized);

        try
        {
            await userAccessTokenService.DeleteUserAccessTokensAsync(userId.Value, userAccessTokenId, cancellationToken);
            return NoContent();
        }
        catch (UserAccessTokenNotFoundException ex)
        {
            throw new HttpErrorException(StatusCodes.Status404NotFound, ex);
        }
    }

    [HttpGet("me/logout")]
    public IActionResult GetLogout()
    {
        HttpContext.Session.Clear();
        return NoContent();
    }

    [HttpPost("search")]
    public async Task<ActionResult<List<UserSearchResponse>>> PostSearchAsync(
        [FromServices] NaheulbookExecutionContext executionContext,
        SearchUserRequest request,
        CancellationToken cancellationToken
    )
    {
        try
        {
            var users = await userService.SearchUserAsync(executionContext, request.Filter, cancellationToken);
            return users.Select(userMapper.ToSearchResponse).ToList();
        }
        catch (ForbiddenAccessException ex)
        {
            throw new HttpErrorException(StatusCodes.Status403Forbidden, ex);
        }
    }

    [HttpPatch("{UserId:int:min(1)}")]
    public async Task<IActionResult> PatchUserIdAsync(
        [FromServices] NaheulbookExecutionContext executionContext,
        [FromRoute] int userId,
        UpdateUserRequest request,
        CancellationToken cancellationToken
    )
    {
        try
        {
            await userService.UpdateUserAsync(executionContext, userId, request, cancellationToken);
            return NoContent();
        }
        catch (UserNotFoundException ex)
        {
            throw new HttpErrorException(StatusCodes.Status404NotFound, ex);
        }
        catch (ForbiddenAccessException ex)
        {
            throw new HttpErrorException(StatusCodes.Status403Forbidden, ex);
        }
    }
}