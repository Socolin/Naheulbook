using FluentAssertions;
using Naheulbook.Core.Features.Users;
using NUnit.Framework;

namespace Naheulbook.Core.Tests.Unit.Features.User;

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