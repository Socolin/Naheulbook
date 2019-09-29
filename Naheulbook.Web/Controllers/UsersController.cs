using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Naheulbook.Core.Exceptions;
using Naheulbook.Core.Models;
using Naheulbook.Core.Services;
using Naheulbook.Requests.Requests;
using Naheulbook.Web.Exceptions;
using Naheulbook.Web.Extensions;
using Naheulbook.Web.Responses;
using Naheulbook.Web.Services;

namespace Naheulbook.Web.Controllers
{
    [Route("api/v2/users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IJwtService _jwtService;
        private readonly IMapper _mapper;

        public UsersController(IUserService userService, IJwtService jwtService, IMapper mapper)
        {
            _userService = userService;
            _jwtService = jwtService;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<StatusCodeResult> PostAsync(CreateUserRequest request)
        {
            try
            {
                await _userService.CreateUserAsync(request.Username, request.Password);
            }
            catch (UserAlreadyExistsException)
            {
                return Conflict();
            }

            return StatusCode(StatusCodes.Status201Created);
        }

        [HttpPost("{Username}/validate")]
        public async Task<StatusCodeResult> PostValidateUserAsync(string username, ValidateUserRequest request)
        {
            try
            {
                await _userService.ValidateUserAsync(username, request.ActivationCode);
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
        public async Task<ActionResult<UserJwtResponse>> PostGenerateJwtAsync(string username, GenerateJwtRequest request)
        {
            try
            {
                var user = await _userService.CheckPasswordAsync(username, request.Password);
                HttpContext.Session.SetCurrentUserId(user.Id);
                var token = _jwtService.GenerateJwtToken(user.Id);
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
        public async Task<ActionResult<UserInfoResponse>> GetCurrentUserInfoAsync()
        {
            // FIXME: userSession if userId is not found, check in db if long duration session still valid
            var userId = HttpContext.Session.GetCurrentUserId();
            if (!userId.HasValue)
                return StatusCode(StatusCodes.Status401Unauthorized);

            var user = await _userService.GetUserInfoAsync(userId.Value);
            return _mapper.Map<UserInfoResponse>(user);
        }

        [HttpGet("me/jwt")]
        public async Task<ActionResult<UserJwtResponse>> GetAJwtAsync()
        {
            // FIXME: userSession if userId is not found, check in db if long duration session still valid
            var userId = HttpContext.Session.GetCurrentUserId();
            if (!userId.HasValue)
                return StatusCode(StatusCodes.Status401Unauthorized);

            var userInfo = await _userService.GetUserInfoAsync(userId.Value);
            var token = _jwtService.GenerateJwtToken(userId.Value);

            return new UserJwtResponse
            {
                Token = token,
                UserInfo = _mapper.Map<UserInfoResponse>(userInfo)
            };
        }

        [HttpGet("me/logout")]
        public IActionResult GetLogout()
        {
            HttpContext.Session.Clear();
            return NoContent();
        }


        [HttpPatch("{UserId:int:min(1)}")]
        public async Task<IActionResult> PatchUserIdAsync(
            [FromServices] NaheulbookExecutionContext executionContext,
            [FromRoute] int userId,
            UpdateUserRequest request
        )
        {
            try
            {
                await _userService.UpdateUserAsync(executionContext, userId, request);
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
}