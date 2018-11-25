using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Naheulbook.Core.Exceptions;
using Naheulbook.Core.Services;
using Naheulbook.Web.Requests;

namespace Naheulbook.Web.Controllers
{
    [Route("api/v2/users")]
    [ApiController]
    public class UsersController : Controller
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
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
    }
}