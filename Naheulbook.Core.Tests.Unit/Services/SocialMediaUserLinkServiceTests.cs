using System.Threading.Tasks;
using FluentAssertions;
using Naheulbook.Core.Services;
using Naheulbook.Core.Tests.Unit.TestUtils;
using Naheulbook.Data.Models;
using NSubstitute;
using NUnit.Framework;

namespace Naheulbook.Core.Tests.Unit.Services;

public class SocialMediaUserLinkServiceTests
{
    private SocialMediaUserLinkService _service;
    private FakeUnitOfWorkFactory _fakeUnitOfWorkFactory;

    [SetUp]
    public void SetUp()
    {
        _fakeUnitOfWorkFactory = new FakeUnitOfWorkFactory();

        _service = new SocialMediaUserLinkService(_fakeUnitOfWorkFactory);
    }

    [Test]
    public async Task GetOrCreateUserFromFacebookAsync_ShouldReturnAlreadyExistingUser()
    {
        const string facebookId = "some-facebook-id";
        var expectedUser = new UserEntity();

        _fakeUnitOfWorkFactory.GetUnitOfWork().Users.GetByFacebookIdAsync(facebookId)
            .Returns(expectedUser);

        var actual = await _service.GetOrCreateUserFromFacebookAsync("some-name", facebookId);

        actual.Should().BeSameAs(expectedUser);
        await _fakeUnitOfWorkFactory.GetUnitOfWork().DidNotReceive().SaveChangesAsync();
    }

    [Test]
    public async Task GetOrCreateUserFromFacebookAsync_ShouldCreateUserWhenItDoesNotExists()
    {
        const string facebookId = "some-facebook-id";

        _fakeUnitOfWorkFactory.GetUnitOfWork().Users.GetByFacebookIdAsync(facebookId)
            .Returns((UserEntity) null);

        var actual = await _service.GetOrCreateUserFromFacebookAsync("some-name", facebookId);

        var userRepository = _fakeUnitOfWorkFactory.GetUnitOfWork().Users;
        Received.InOrder(() =>
        {
            userRepository.Add(actual);
            _fakeUnitOfWorkFactory.GetUnitOfWork().SaveChangesAsync();
        });
        actual.Should().BeEquivalentTo(new UserEntity
        {
            FbId = facebookId,
            Admin = false,
            DisplayName = "some-name"
        });
    }

    [Test]
    public async Task GetOrCreateUserFromGoogleAsync_ShouldReturnAlreadyExistingUser()
    {
        const string googleId = "some-google-id";
        var expectedUser = new UserEntity();

        _fakeUnitOfWorkFactory.GetUnitOfWork().Users.GetByGoogleIdAsync(googleId)
            .Returns(expectedUser);

        var actual = await _service.GetOrCreateUserFromGoogleAsync("some-name", googleId);

        actual.Should().BeSameAs(expectedUser);
        await _fakeUnitOfWorkFactory.GetUnitOfWork().DidNotReceive().SaveChangesAsync();
    }

    [Test]
    public async Task GetOrCreateUserFromGoogleAsync_ShouldCreateUserWhenItDoesNotExists()
    {
        const string googleId = "some-google-id";

        _fakeUnitOfWorkFactory.GetUnitOfWork().Users.GetByGoogleIdAsync(googleId)
            .Returns((UserEntity) null);

        var actual = await _service.GetOrCreateUserFromGoogleAsync("some-name", googleId);

        var userRepository = _fakeUnitOfWorkFactory.GetUnitOfWork().Users;
        Received.InOrder(() =>
        {
            userRepository.Add(actual);
            _fakeUnitOfWorkFactory.GetUnitOfWork().SaveChangesAsync();
        });
        actual.Should().BeEquivalentTo(new UserEntity
        {
            GoogleId = googleId,
            Admin = false,
            DisplayName = "some-name"
        });
    }

    [Test]
    public async Task GetOrCreateUserFromTwitterAsync_ShouldReturnAlreadyExistingUser()
    {
        const string twitterId = "some-twitter-id";
        var expectedUser = new UserEntity();

        _fakeUnitOfWorkFactory.GetUnitOfWork().Users.GetByTwitterIdAsync(twitterId)
            .Returns(expectedUser);

        var actual = await _service.GetOrCreateUserFromTwitterAsync("some-name", twitterId);

        actual.Should().BeSameAs(expectedUser);
        await _fakeUnitOfWorkFactory.GetUnitOfWork().DidNotReceive().SaveChangesAsync();
    }

    [Test]
    public async Task GetOrCreateUserFromTwitterAsync_ShouldCreateUserWhenItDoesNotExists()
    {
        const string twitterId = "some-twitter-id";

        _fakeUnitOfWorkFactory.GetUnitOfWork().Users.GetByTwitterIdAsync(twitterId)
            .Returns((UserEntity) null);

        var actual = await _service.GetOrCreateUserFromTwitterAsync("some-name", twitterId);

        var userRepository = _fakeUnitOfWorkFactory.GetUnitOfWork().Users;
        Received.InOrder(() =>
        {
            userRepository.Add(actual);
            _fakeUnitOfWorkFactory.GetUnitOfWork().SaveChangesAsync();
        });
        actual.Should().BeEquivalentTo(new UserEntity
        {
            TwitterId = twitterId,
            Admin = false,
            DisplayName = "some-name"
        });
    }

    [Test]
    public async Task GetOrCreateUserFromMicrosoftAsync_ShouldReturnAlreadyExistingUser()
    {
        const string microsoftId = "some-microsoft-id";
        var expectedUser = new UserEntity();

        _fakeUnitOfWorkFactory.GetUnitOfWork().Users.GetByMicrosoftIdAsync(microsoftId)
            .Returns(expectedUser);

        var actual = await _service.GetOrCreateUserFromMicrosoftAsync("some-name", microsoftId);

        actual.Should().BeSameAs(expectedUser);
        await _fakeUnitOfWorkFactory.GetUnitOfWork().DidNotReceive().SaveChangesAsync();
    }

    [Test]
    public async Task GetOrCreateUserFromMicrosoftAsync_ShouldCreateUserWhenItDoesNotExists()
    {
        const string microsoftId = "some-microsoft-id";

        _fakeUnitOfWorkFactory.GetUnitOfWork().Users.GetByMicrosoftIdAsync(microsoftId)
            .Returns((UserEntity) null);

        var actual = await _service.GetOrCreateUserFromMicrosoftAsync("some-name", microsoftId);

        var userRepository = _fakeUnitOfWorkFactory.GetUnitOfWork().Users;
        Received.InOrder(() =>
        {
            userRepository.Add(actual);
            _fakeUnitOfWorkFactory.GetUnitOfWork().SaveChangesAsync();
        });
        actual.Should().BeEquivalentTo(new UserEntity
        {
            MicrosoftId = microsoftId,
            Admin = false,
            DisplayName = "some-name"
        });
    }
}