using System.Threading.Tasks;
using FluentAssertions;
using Naheulbook.Core.Services;
using Naheulbook.Core.Tests.Unit.TestUtils;
using Naheulbook.Data.Models;
using NSubstitute;
using NUnit.Framework;

namespace Naheulbook.Core.Tests.Unit.Services
{
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
            var expectedUser = new User();

            _fakeUnitOfWorkFactory.GetUnitOfWork().Users.GetByFacebookIdAsync(facebookId)
                .Returns(expectedUser);

            var actual = await _service.GetOrCreateUserFromFacebookAsync("some-name", facebookId);

            actual.Should().BeSameAs(expectedUser);
            await _fakeUnitOfWorkFactory.GetUnitOfWork().DidNotReceive().CompleteAsync();
        }

        [Test]
        public async Task GetOrCreateUserFromFacebookAsync_ShouldCreateUserWhenItDoesNotExists()
        {
            const string facebookId = "some-facebook-id";

            _fakeUnitOfWorkFactory.GetUnitOfWork().Users.GetByFacebookIdAsync(facebookId)
                .Returns((User) null);

            var actual = await _service.GetOrCreateUserFromFacebookAsync("some-name", facebookId);

            Received.InOrder(() =>
            {
                _fakeUnitOfWorkFactory.GetUnitOfWork().Users.Add(actual);
                _fakeUnitOfWorkFactory.GetUnitOfWork().CompleteAsync();
            });
            actual.Should().BeEquivalentTo(new User
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
            var expectedUser = new User();

            _fakeUnitOfWorkFactory.GetUnitOfWork().Users.GetByGoogleIdAsync(googleId)
                .Returns(expectedUser);

            var actual = await _service.GetOrCreateUserFromGoogleAsync("some-name", googleId);

            actual.Should().BeSameAs(expectedUser);
            await _fakeUnitOfWorkFactory.GetUnitOfWork().DidNotReceive().CompleteAsync();
        }

        [Test]
        public async Task GetOrCreateUserFromGoogleAsync_ShouldCreateUserWhenItDoesNotExists()
        {
            const string googleId = "some-google-id";

            _fakeUnitOfWorkFactory.GetUnitOfWork().Users.GetByGoogleIdAsync(googleId)
                .Returns((User) null);

            var actual = await _service.GetOrCreateUserFromGoogleAsync("some-name", googleId);

            Received.InOrder(() =>
            {
                _fakeUnitOfWorkFactory.GetUnitOfWork().Users.Add(actual);
                _fakeUnitOfWorkFactory.GetUnitOfWork().CompleteAsync();
            });
            actual.Should().BeEquivalentTo(new User
            {
                GoogleId = googleId,
                Admin = false,
                DisplayName = "some-name"
            });
        }
    }
}