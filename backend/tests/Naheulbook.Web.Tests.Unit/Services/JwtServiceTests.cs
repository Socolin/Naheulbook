using System.Text;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.Extensions.Options;
using Naheulbook.Data.EntityFrameworkCore.Entities;
using Naheulbook.Shared.Utils;
using Naheulbook.Web.Configurations;
using Naheulbook.Web.Services;
using Newtonsoft.Json.Linq;
using NSubstitute;
using NUnit.Framework;

namespace Naheulbook.Web.Tests.Unit.Services;

public class JwtServiceTests
{
    private JwtService _jwtService;
    private ITimeService _timeService;

    [SetUp]
    public void SetUp()
    {
        var authenticationConfiguration = new AuthenticationOptions()
        {
            JwtSigningKey = Convert.ToBase64String("some-key"u8.ToArray()),
            JwtExpirationDelayInMinutes = 3,
        };

        _timeService = Substitute.For<ITimeService>();
        _jwtService = new JwtService(Options.Create(authenticationConfiguration), _timeService);
    }

    [Test]
    public void CanGenerateATokenUsingConfiguration()
    {
        var user = new UserEntity {Id = 42};
        var now = new DateTimeOffset(new DateTime(2018, 8, 4, 4, 42, 12, DateTimeKind.Utc));

        _timeService.UtcNow
            .Returns(now);

        var token = _jwtService.GenerateJwtToken(user.Id);

        using (new AssertionScope())
        {
            var tokenElements = token.Split('.');
            tokenElements.Length.Should().Be(3);

            var header = JObject.Parse(Encoding.UTF8.GetString(Convert.FromBase64String(tokenElements[0])));
            header.Value<string>("alg").Should().Be("HS256");
            header.Value<string>("typ").Should().Be("JWT");

            var payload = JObject.Parse(Encoding.UTF8.GetString(Convert.FromBase64String(tokenElements[1])));
            payload.Value<string>("sub").Should().Be("42");
            payload.Value<long>("exp").Should().Be(now.AddMinutes(3).ToUnixTimeSeconds());
        }
    }
}