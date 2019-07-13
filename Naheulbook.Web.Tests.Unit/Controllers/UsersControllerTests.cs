using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Naheulbook.Core.Exceptions;
using Naheulbook.Core.Services;
using Naheulbook.Data.Models;
using Naheulbook.Requests.Requests;
using Naheulbook.Web.Controllers;
using Naheulbook.Web.Services;
using NSubstitute;
using NUnit.Framework;

namespace Naheulbook.Web.Tests.Unit.Controllers
{
    public class UsersControllerTests
    {
        private const string SomeUsername = "some-username";
        private const string SomePassword = "some-password";
        private const string SomeActivationCode = "some-activation-code";
        private IUserService _userService;
        private IJwtService _jwtService;
        private UsersController _usersController;
        private ISession _session;
        private HttpContext _httpContext;

        [SetUp]
        public void SetUp()
        {
            _userService = Substitute.For<IUserService>();
            _jwtService = Substitute.For<IJwtService>();
            _session = Substitute.For<ISession>();
            _httpContext = Substitute.For<HttpContext>();
            _httpContext.Session.Returns(_session);

            _usersController = new UsersController(_userService, _jwtService, Substitute.For<IMapper>())
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = _httpContext
                }
            };
        }

        [Test]
        public async Task WhenCreatingUser_Return201Created()
        {
            var response = await _usersController.PostAsync(new CreateUserRequest());

            response.StatusCode.Should().Be((int) HttpStatusCode.Created);
        }

        [Test]
        public async Task WhenCreatingUser_CallUserService()
        {
            var request = CreateUserRequest();

            await _usersController.PostAsync(request);

            await _userService.Received(1)
                .CreateUserAsync(SomeUsername, SomePassword);
        }

        [Test]
        public async Task WhenCreatingUser_AndUserAlreadyExists_Return409Conflict()
        {
            var request = CreateUserRequest();

            _userService.CreateUserAsync(SomeUsername, SomePassword)
                .Returns(Task.FromException(new UserAlreadyExistsException()));

            var response = await _usersController.PostAsync(request);

            response.StatusCode.Should().Be((int) HttpStatusCode.Conflict);
        }

        [Test]
        public async Task WhenPostToValidateUser_AndItSucceed_Return204NoContent()
        {
            var validateUserRequest = ValidateUserRequest();

            var response = await _usersController.PostValidateUserAsync(SomeUsername, validateUserRequest);

            response.StatusCode.Should().Be((int) HttpStatusCode.NoContent);
        }

        [Test]
        public async Task WhenPostToValidateUser_CallUserService()
        {
            var validateUserRequest = ValidateUserRequest();

            await _usersController.PostValidateUserAsync(SomeUsername, validateUserRequest);

            await _userService.Received(1)
                .ValidateUserAsync(SomeUsername, SomeActivationCode);
        }

        [Test]
        public async Task WhenPostToValidateUser_AndUserDoesNotExists_Return403()
        {
            var validateUserRequest = ValidateUserRequest();

            _userService.ValidateUserAsync(SomeUsername, SomeActivationCode)
                .Returns(Task.FromException(new UserNotFoundException()));

            var response = await _usersController.PostValidateUserAsync(SomeUsername, validateUserRequest);

            response.StatusCode.Should().Be((int) HttpStatusCode.Forbidden);
        }

        [Test]
        public async Task WhenPostToValidateUser_AndCodeIsInvalid_Return403()
        {
            var validateUserRequest = ValidateUserRequest();

            _userService.ValidateUserAsync(SomeUsername, SomeActivationCode)
                .Returns(Task.FromException(new InvalidUserActivationCodeException()));

            var response = await _usersController.PostValidateUserAsync(SomeUsername, validateUserRequest);

            response.StatusCode.Should().Be((int) HttpStatusCode.Forbidden);
        }

        [Test]
        public async Task WhenPostToGenerateUserJwt_CheckPassword_AndGenerateAJwtUsingUserId()
        {
            var generateJwtRequest = new GenerateJwtRequest {Password = SomePassword};
            var user = new User();

            _userService.CheckPasswordAsync(SomeUsername, SomePassword)
                .Returns(user);
            _jwtService.GenerateJwtToken(user.Id)
                .Returns("some-jwt");

            var response = await _usersController.PostGenerateJwtAsync(SomeUsername, generateJwtRequest);

            response.Value.Token.Should().Be("some-jwt");
        }

        [Test]
        public async Task WhenPostToGenerateUserJwt_SaveCurrentUserIdToSession()
        {
            var generateJwtRequest = new GenerateJwtRequest {Password = SomePassword};
            var user = new User
            {
                Id = 1
            };

            _userService.CheckPasswordAsync(SomeUsername, SomePassword)
                .Returns(user);
            _jwtService.GenerateJwtToken(user.Id)
                .Returns("some-jwt");

            await _usersController.PostGenerateJwtAsync(SomeUsername, generateJwtRequest);

            _session.Received(1).Set("userId", Arg.Is<byte[]>(x => x.SequenceEqual(new byte[] {0, 0, 0, 1})));
        }

        [Test]
        public async Task WhenPostToGenerateUserJwt_AndPasswordIsInvalid_Return401()
        {
            var generateJwtRequest = new GenerateJwtRequest {Password = SomePassword};

            _userService.CheckPasswordAsync(SomeUsername, SomePassword)
                .Returns(Task.FromException<User>(new InvalidPasswordException()));

            var response = await _usersController.PostGenerateJwtAsync(SomeUsername, generateJwtRequest);

            response.Result.Should().BeOfType<StatusCodeResult>().Subject.StatusCode.Should().Be((int) HttpStatusCode.Unauthorized);
        }

        [Test]
        public async Task WhenPostToGenerateUserJwt_AndUsernameIsInvalid_Return401()
        {
            var generateJwtRequest = new GenerateJwtRequest {Password = SomePassword};

            _userService.CheckPasswordAsync(SomeUsername, SomePassword)
                .Returns(Task.FromException<User>(new UserNotFoundException()));

            var response = await _usersController.PostGenerateJwtAsync(SomeUsername, generateJwtRequest);

            response.Result.Should().BeOfType<StatusCodeResult>().Subject.StatusCode.Should().Be((int) HttpStatusCode.Unauthorized);
        }

        private static ValidateUserRequest ValidateUserRequest()
        {
            return new ValidateUserRequest()
            {
                ActivationCode = SomeActivationCode
            };
        }

        private static CreateUserRequest CreateUserRequest()
        {
            return new CreateUserRequest
            {
                Username = SomeUsername,
                Password = SomePassword
            };
        }
    }
}