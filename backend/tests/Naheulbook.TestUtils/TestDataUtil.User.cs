using Naheulbook.Data.EntityFrameworkCore.Entities;
using Naheulbook.Shared.Extensions;

namespace Naheulbook.TestUtils;

public partial class TestDataUtil
{
    public TestDataUtil AddUser(Action<UserEntity> customizer = null)
    {
        return AddUser(out _, customizer);
    }

    public TestDataUtil AddUser(out UserEntity user, Action<UserEntity> customizer = null)
    {
        var suffix = RngUtil.GetRandomHexString(8);

        user = new UserEntity
        {
            Username = $"some-username-{suffix}",
            DisplayName = $"some-display-name-{suffix}",
            HashedPassword = $"some-hashed-password-{suffix}",
            ActivationCode = "some-activation-code",
            FbId = $"some-fb-id-{suffix}",
            GoogleId = $"some-google-id-{suffix}",
            TwitterId = $"some-twitter-id-{suffix}",
            MicrosoftId = $"some-microsoft-id-{suffix}",
            ShowInSearchUntil = DateTime.UtcNow.RoundToSeconds(),
        };

        return SaveEntity(user, customizer);
    }

    public TestDataUtil AddUserAccessToken(Action<UserAccessTokenEntity> customizer = null)
    {
        return AddUserAccessToken(out _, customizer);
    }

    public TestDataUtil AddUserAccessToken(out UserAccessTokenEntity userAccessToken, Action<UserAccessTokenEntity> customizer = null)
    {
        userAccessToken = new UserAccessTokenEntity
        {
            Id = Guid.NewGuid(),
            Name = RngUtil.GetRandomString("some-token-name"),
            Key = RngUtil.GetRandomHexString(10),
            UserId = GetLast<UserEntity>().Id,
            DateCreated = DateTimeOffset.Now.ToUniversalTime(),
        };

        return SaveEntity(userAccessToken, customizer);
    }
}