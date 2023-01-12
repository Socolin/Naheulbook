using System;
using Naheulbook.Data.Models;

// ReSharper disable MemberCanBeMadeStatic.Global

namespace Naheulbook.TestUtils;

public partial class DefaultEntityCreator
{
    public UserEntity CreateUser(string suffix = null)
    {
        if (suffix == null)
            suffix = RngUtil.GetRandomHexString(8);

        return new UserEntity
        {
            Username = $"some-username-{suffix}",
            DisplayName = $"some-display-name-{suffix}",
            HashedPassword = $"some-hashed-password-{suffix}",
            ActivationCode = "some-activation-code",
            FbId = $"some-fb-id-{suffix}",
            GoogleId = $"some-google-id-{suffix}",
            TwitterId = $"some-twitter-id-{suffix}",
            MicrosoftId = $"some-microsoft-id-{suffix}",
        };
    }

    public UserAccessTokenEntity CreateUserAccessToken(UserEntity user, string suffix = null)
    {
        if (suffix == null)
            suffix = RngUtil.GetRandomHexString(8);

        return new UserAccessTokenEntity
        {
            Id = Guid.NewGuid(),
            Name = $"some-token-name-{suffix}",
            Key = RngUtil.GetRandomHexString(10),
            UserId = user.Id,
            DateCreated = DateTimeOffset.Now.ToUniversalTime(),
        };
    }
}