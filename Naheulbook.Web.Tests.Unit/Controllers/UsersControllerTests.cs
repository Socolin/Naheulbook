using System;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Naheulbook.Core.Exceptions;
using Naheulbook.Core.Services;
using Naheulbook.Web.Controllers;
using Naheulbook.Web.Requests;
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
        private UsersController _usersController;

        [SetUp]
        public void SetUp()
        {
            _userService = Substitute.For<IUserService>();
            _usersController = new UsersController(_userService);
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