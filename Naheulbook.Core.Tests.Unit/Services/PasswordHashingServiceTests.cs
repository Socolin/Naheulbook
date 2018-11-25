using FluentAssertions;
using Naheulbook.Core.Services;
using NUnit.Framework;

namespace Naheulbook.Core.Tests.Unit.Services
{
    public class PasswordHashingServiceTests
    {
        private PasswordHashingService _passwordHashingService;

        [SetUp]
        public void SetUp()
        {
            _passwordHashingService = new PasswordHashingService();
        }

        [Test]
        public void CanHashPassword_ThenVerifyIt()
        {
            const string password = "some-password";

            var hashedPassword = _passwordHashingService.HashPassword(password);
            var passwordsMatches = _passwordHashingService.VerifyPassword(hashedPassword, password);

            passwordsMatches.Should().BeTrue();
        }
    }
}