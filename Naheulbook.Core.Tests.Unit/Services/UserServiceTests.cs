using System;
using System.Threading.Tasks;
using FluentAssertions;
using Naheulbook.Core.Exceptions;
using Naheulbook.Core.Services;
using Naheulbook.Data.Factories;
using Naheulbook.Data.Models;
using Naheulbook.Data.Repositories;
using Naheulbook.Data.UnitOfWorks;
using NSubstitute;
using NUnit.Framework;

namespace Naheulbook.Core.Tests.Unit.Services
{
    public class UserServiceTests
    {
        private const string SomePassword = "some-password";
        private const string SomeUsername = "some-username";
        private const string SomeActivationCode = "some-activation-code";
        private const string SomeEncryptedPassword = "some-encrypted-password";

        private IUserRepository _userRepository;
        private UserService _userService;
        private IUnitOfWork _unitOfWork;
        private IMailService _mailService;
        private IPasswordHashingService _passwordHashingService;

        [SetUp]
        public void SetUp()
        {
            var unitOfWorkFactory = Substitute.For<IUnitOfWorkFactory>();
            _unitOfWork = Substitute.For<IUnitOfWork>();
            unitOfWorkFactory.CreateUnitOfWork().Returns(_unitOfWork);
            _userRepository = Substitute.For<IUserRepository>();
            _unitOfWork.Users.Returns(_userRepository);

            _mailService = Substitute.For<IMailService>();
            _passwordHashingService = Substitute.For<IPasswordHashingService>();

            _userService = new UserService(unitOfWorkFactory, _passwordHashingService, _mailService);
        }

        [Test]
        public async Task WhenCreatingUser_CreateUserInDatabase_AndHashPassword()
        {
            _passwordHashingService.HashPassword(SomePassword)
                .Returns(SomeEncryptedPassword);

            await _userService.CreateUserAsync(SomeUsername, SomePassword);

            Received.InOrder(() =>
            {
                _userRepository.Add(Arg.Is<User>(u =>
                    u.Username == SomeUsername
                    && u.HashedPassword == SomeEncryptedPassword
                    && !string.IsNullOrEmpty(u.ActivationCode))
                );
                _unitOfWork.CompleteAsync();
            });
        }
        [Test]
        public void WhenCreatingUser_AndUserExists_ThenThrows()
        {
            _userRepository.GetByUsernameAsync(SomeUsername)
                .Returns(new User());

            Func<Task> act = () => _userService.CreateUserAsync(SomeUsername, SomePassword);

            act.Should().Throw<UserAlreadyExistsException>();
        }


        [Test]
        public async Task WhenCreatingUser_SendMailWithActivationCode()
        {
            _passwordHashingService.HashPassword(SomePassword)
                .Returns(SomeEncryptedPassword);

            await _userService.CreateUserAsync(SomeUsername, SomePassword);

            await _mailService.Received(1)
                .SendCreateUserMailAsync(SomeUsername, Arg.Is<string>(s => !string.IsNullOrEmpty(s)));
        }

        [Test]
        public async Task WhenValidatingUser_AndValidationCodeIsValid_SetValidationCodeToNull()
        {
            var user = CreateUser();

            _userRepository.GetByUsernameAsync(SomeUsername)
                .Returns(user);

            await _userService.ValidateUserAsync(SomeUsername, SomeActivationCode);

            user.ActivationCode.Should().BeNull();
            await _unitOfWork.Received(1).CompleteAsync();
        }

        [Test]
        public void WhenValidatingUser_AndValidationCodeIsInvalid_Throw()
        {
            var user = CreateUser();

            _userRepository.GetByUsernameAsync(SomeUsername)
                .Returns(user);

            Func<Task> act = async () => await _userService.ValidateUserAsync(SomeUsername, "some-invalid-code");

            act.Should().Throw<InvalidUserActivationCodeException>();
        }

        [Test]
        public void WhenValidatingUser_AndUserNotFound_Throw()
        {
            _userRepository.GetByUsernameAsync(SomeUsername)
                .Returns((User) null);

            Func<Task> act = async () => await _userService.ValidateUserAsync(SomeUsername, SomeActivationCode);

            act.Should().Throw<UserNotFoundException>();
        }


        private static User CreateUser()
        {
            return new User()
            {
                Username = SomeUsername,
                ActivationCode = SomeActivationCode
            };
        }
    }
}