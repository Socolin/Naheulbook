using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Naheulbook.Core.Exceptions;
using Naheulbook.Core.Services;
using Naheulbook.Requests.Requests;
using Naheulbook.Web.Extensions;
using Naheulbook.Web.Responses;
using Naheulbook.Web.Services;

namespace Naheulbook.Web.Controllers
{
    [Route("api/v2/users")]
    [ApiController]
    public class UsersController : Controller
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

            return StatusCode((int) HttpStatusCode.Created);
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
                return StatusCode((int) HttpStatusCode.Forbidden);
            }
            catch (InvalidUserActivationCodeException)
            {
                return StatusCode((int) HttpStatusCode.Forbidden);
            }

            return StatusCode((int) HttpStatusCode.NoContent);
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
                return StatusCode((int) HttpStatusCode.Unauthorized);
            }
            catch (InvalidPasswordException)
            {
                return StatusCode((int) HttpStatusCode.Unauthorized);
            }
        }

        [HttpGet("me")]
        public async Task<ActionResult<UserInfoResponse>> GetCurrentUserInfo()
        {
            // FIXME: userSession if userId is not found, check in db if long duration session still valid
            var userId = HttpContext.Session.GetCurrentUserId();
            if (!userId.HasValue)
                return StatusCode((int) HttpStatusCode.Unauthorized);

            var user = await _userService.GetUserInfoAsync(userId.Value);
            return _mapper.Map<UserInfoResponse>(user);
        }

        [HttpPost("jwt")]
        public async Task<ActionResult<UserJwtResponse>> PostGetAJwt()
        {
            // FIXME: userSession if userId is not found, check in db if long duration session still valid
            var userId = HttpContext.Session.GetCurrentUserId();
            if (!userId.HasValue)
                return StatusCode((int) HttpStatusCode.Unauthorized);

            var userInfo = await _userService.GetUserInfoAsync(userId.Value);
            var token = _jwtService.GenerateJwtToken(userId.Value);

            return new UserJwtResponse
            {
                Token = token,
                UserInfo = _mapper.Map<UserInfoResponse>(userInfo)
            };
        }
    }
}