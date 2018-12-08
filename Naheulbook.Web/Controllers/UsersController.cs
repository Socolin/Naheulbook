using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Naheulbook.Core.Exceptions;
using Naheulbook.Core.Services;
using Naheulbook.Web.Requests;
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

        public UsersController(IUserService userService, IJwtService jwtService)
        {
            _userService = userService;
            _jwtService = jwtService;
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
                var token = _jwtService.GenerateJwtToken(user.Id);
                return new UserJwtResponse() {Token = token};
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
    }
}