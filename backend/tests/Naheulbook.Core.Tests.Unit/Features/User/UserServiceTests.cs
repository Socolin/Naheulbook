using FluentAssertions;
using Naheulbook.Core.Features.Shared;
using Naheulbook.Core.Features.Users;
using Naheulbook.Core.Tests.Unit.TestUtils;
using Naheulbook.Data.EntityFrameworkCore.Entities;
using NSubstitute;
using NUnit.Framework;

namespace Naheulbook.Core.Tests.Unit.Features.User;

public class UserServiceTests
{
    private const string SomePassword = "some-password";
    private const string SomeUsername = "some-username";
    private const string SomeActivationCode = "some-activation-code";
    private const string SomeEncryptedPassword = "some-encrypted-password";

    private FakeUnitOfWorkFactory _unitOfWorkFactory;
    private IMailService _mailService;
    private IPasswordHashingService _passwordHashingService;
    private IAuthorizationUtil _authorizationUtil;

    private UserService _service;

    [SetUp]
    public void SetUp()
    {
        _unitOfWorkFactory = new FakeUnitOfWorkFactory();
        _mailService = Substitute.For<IMailService>();
        _passwordHashingService = Substitute.For<IPasswordHashingService>();
        _authorizationUtil = Substitute.For<IAuthorizationUtil>();

        _service = new UserService(
            _unitOfWorkFactory,
            _passwordHashingService,
            _mailService,
            _authorizationUtil
        );
    }

    [Test]
    public async Task WhenCreatingUser_CreateUserInDatabase_AndHashPassword()
    {
        _passwordHashingService.HashPassword(SomePassword)
            .Returns(SomeEncryptedPassword);

        await _service.CreateUserAsync(SomeUsername, SomePassword);


        var userRepository = _unitOfWorkFactory.GetUnitOfWork().Users;
        Received.InOrder(() =>
        {
            userRepository.Add(Arg.Is<UserEntity>(u =>
                u.Username == SomeUsername
                && u.HashedPassword == SomeEncryptedPassword
                && !string.IsNullOrEmpty(u.ActivationCode))
            );
            _unitOfWorkFactory.GetUnitOfWork().SaveChangesAsync();
        });
    }
    [Test]
    public async Task WhenCreatingUser_AndUserExists_ThenThrows()
    {
        _unitOfWorkFactory.GetUnitOfWork().Users.GetByUsernameAsync(SomeUsername)
            .Returns(new UserEntity());

        Func<Task> act = () => _service.CreateUserAsync(SomeUsername, SomePassword);

        await act.Should().ThrowAsync<UserAlreadyExistsException>();
    }


    [Test]
    public async Task WhenCreatingUser_SendMailWithActivationCode()
    {
        await _service.CreateUserAsync(SomeUsername, SomePassword);

        await _mailService.Received(1)
            .SendCreateUserMailAsync(SomeUsername, Arg.Is<string>(s => !string.IsNullOrEmpty(s)));
    }

    [Test]
    public async Task WhenValidatingUser_AndValidationCodeIsValid_SetValidationCodeToNull()
    {
        var user = CreateUser();

        _unitOfWorkFactory.GetUnitOfWork().Users.GetByUsernameAsync(SomeUsername)
            .Returns(user);

        await _service.ValidateUserAsync(SomeUsername, SomeActivationCode);

        user.ActivationCode.Should().BeNull();
        await _unitOfWorkFactory.GetUnitOfWork().Received(1).SaveChangesAsync();
    }

    [Test]
    public async Task WhenValidatingUser_AndValidationCodeIsInvalid_Throw()
    {
        var user = CreateUser();

        _unitOfWorkFactory.GetUnitOfWork().Users.GetByUsernameAsync(SomeUsername)
            .Returns(user);

        Func<Task> act = async () => await _service.ValidateUserAsync(SomeUsername, "some-invalid-code");

        await act.Should().ThrowAsync<InvalidUserActivationCodeException>();
    }

    [Test]
    public async Task WhenValidatingUser_AndUserNotFound_Throw()
    {
        _unitOfWorkFactory.GetUnitOfWork().Users.GetByUsernameAsync(SomeUsername)
            .Returns((UserEntity) null);

        Func<Task> act = async () => await _service.ValidateUserAsync(SomeUsername, SomeActivationCode);

        await act.Should().ThrowAsync<UserNotFoundException>();
    }

    [Test]
    public async Task WhenCheckingPassword_CallPasswordEncryptionService()
    {
        const string someHashedPassword = "some-hashed-password";
        var user = new UserEntity {Username = SomeUsername, HashedPassword = someHashedPassword};

        _unitOfWorkFactory.GetUnitOfWork().Users.GetByUsernameAsync(SomeUsername)
            .Returns(user);
        _passwordHashingService.VerifyPassword(someHashedPassword, SomePassword)
            .Returns(true);

        var result = await _service.CheckPasswordAsync(SomeUsername, SomePassword);

        result.Should().Be(user);
    }

    [Test]
    public async Task WhenCheckingPassword_AndPasswordIsInvalid_Throw()
    {
        var user = new UserEntity {Username = SomeUsername, HashedPassword = "some-hashed-password"};

        _unitOfWorkFactory.GetUnitOfWork().Users.GetByUsernameAsync(SomeUsername)
            .Returns(user);
        _passwordHashingService.VerifyPassword("some-hashed-password", SomePassword)
            .Returns(false);

        Func<Task> act = async () => await _service.CheckPasswordAsync(SomeUsername, SomePassword);

        await act.Should().ThrowAsync<InvalidPasswordException>();
    }

    [Test]
    public async Task WhenCheckingPassword_AndUserDoesNotExists_Throw()
    {
        _unitOfWorkFactory.GetUnitOfWork().Users.GetByUsernameAsync(SomeUsername)
            .Returns((UserEntity) null);

        Func<Task> act = async () => await _service.CheckPasswordAsync(SomeUsername, SomePassword);

        await act.Should().ThrowAsync<UserNotFoundException>();
    }

    private static UserEntity CreateUser()
    {
        return new UserEntity()
        {
            Username = SomeUsername,
            ActivationCode = SomeActivationCode,
        };
    }
}