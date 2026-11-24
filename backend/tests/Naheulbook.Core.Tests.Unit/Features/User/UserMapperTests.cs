using AwesomeAssertions;
using Naheulbook.Core.Features.Users;
using Naheulbook.Data.EntityFrameworkCore.Entities;
using Naheulbook.Responses.Responses;
using NUnit.Framework;

namespace Naheulbook.Core.Tests.Unit.Features.User;

[TestFixture]
[TestOf(typeof(UserMapper))]
public class UserMapperTests
{
    private UserMapper _mapper;

    [SetUp]
    public void SetUp()
    {
        _mapper = new UserMapper();
    }

    [Test]
    public void ToInfoResponse_User_MapsAllFields()
    {
        var user = new UserEntity
        {
            Id = 3,
            DisplayName = "some-display-name",
            AdminLevel = UserAdminLevel.Admin,
            ShowInSearchUntil = DateTime.UtcNow.AddHours(1),
            FbId = "some-fb-id",
            GoogleId = "some-google-id",
            MicrosoftId = "some-microsoft-id",
        };

        var actual = _mapper.ToInfoResponse(user);

        actual.Should().BeEquivalentTo(new UserInfoResponse
        {
            Id = 3,
            DisplayName = "some-display-name",
            Admin = true,
            AdminLevel = 1,
            ShowInSearch = true,
            LinkedWithFb = true,
            LinkedWithGoogle = true,
            LinkedWithMicrosoft = true,
        });
    }

    [Test]
    public void ToInfoResponse_UserWithoutLinkedAccountsAndSearchExpired_MapsFlagsToFalse()
    {
        var user = new UserEntity
        {
            Id = 3,
            ShowInSearchUntil = DateTime.UtcNow.AddHours(-1),
        };

        var actual = _mapper.ToInfoResponse(user);

        actual.Should().BeEquivalentTo(new UserInfoResponse
        {
            Id = 3,
            ShowInSearch = false,
            LinkedWithFb = false,
            LinkedWithGoogle = false,
            LinkedWithMicrosoft = false,
        });
    }

    [Test]
    public void ToInfoResponse_LinkedWithGoogle_DependsOnGoogleIdOnly()
    {
        var user = new UserEntity {GoogleId = null, FbId = "some-fb-id"};

        var actual = _mapper.ToInfoResponse(user);

        actual.LinkedWithGoogle.Should().BeFalse();
        actual.LinkedWithFb.Should().BeTrue();
    }

    [Test]
    public void ToSearchResponse_User_MapsIdAndDisplayName()
    {
        var user = new UserEntity
        {
            Id = 3,
            DisplayName = "some-display-name",
            Username = "some-username",
        };

        var actual = _mapper.ToSearchResponse(user);

        actual.Should().BeEquivalentTo(new UserSearchResponse
        {
            Id = 3,
            DisplayName = "some-display-name",
        });
    }

    [Test]
    public void ToResponse_UserAccessToken_MapsFieldsWithoutKey()
    {
        var accessToken = CreateAccessToken();

        var actual = _mapper.ToResponse(accessToken);

        actual.Should().BeOfType<UserAccessTokenResponse>();
        actual.Should().BeEquivalentTo(new UserAccessTokenResponse
        {
            Id = accessToken.Id,
            Name = "some-name",
            DateCreated = accessToken.DateCreated,
        });
    }

    [Test]
    public void ToResponseWithKey_UserAccessToken_MapsFieldsWithKey()
    {
        var accessToken = CreateAccessToken();

        var actual = _mapper.ToResponseWithKey(accessToken);

        actual.Should().BeEquivalentTo(new UserAccessTokenResponseWithKey
        {
            Id = accessToken.Id,
            Name = "some-name",
            DateCreated = accessToken.DateCreated,
            Key = "some-key",
        });
    }

    private static UserAccessTokenEntity CreateAccessToken()
    {
        return new UserAccessTokenEntity
        {
            Id = Guid.NewGuid(),
            Name = "some-name",
            Key = "some-key",
            DateCreated = new DateTimeOffset(2024, 3, 4, 5, 6, 7, TimeSpan.Zero),
            UserId = 8,
        };
    }
}
