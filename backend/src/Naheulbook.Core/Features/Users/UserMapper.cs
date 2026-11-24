using Naheulbook.Data.EntityFrameworkCore.Entities;
using Naheulbook.Responses.Responses;

namespace Naheulbook.Core.Features.Users;

public interface IUserMapper
{
    UserInfoResponse ToInfoResponse(UserEntity user);
    UserSearchResponse ToSearchResponse(UserEntity user);
    UserAdminResponse ToAdminResponse(UserEntity user);
    UserAccessTokenResponse ToResponse(UserAccessTokenEntity accessToken);
    UserAccessTokenResponseWithKey ToResponseWithKey(UserAccessTokenEntity accessToken);
}

public class UserMapper : IUserMapper
{
    public UserInfoResponse ToInfoResponse(UserEntity user)
    {
        return new UserInfoResponse
        {
            Id = user.Id,
            DisplayName = user.DisplayName,
            ShowInSearch = user.ShowInSearchUntil > DateTime.UtcNow,
            Admin = user.AdminLevel >= UserAdminLevel.Admin,
            AdminLevel = (int)user.AdminLevel,
            LinkedWithFb = user.FbId != null,
            LinkedWithGoogle = user.GoogleId != null,
            LinkedWithMicrosoft = user.MicrosoftId != null,
        };
    }

    public UserSearchResponse ToSearchResponse(UserEntity user)
    {
        return new UserSearchResponse
        {
            Id = user.Id,
            DisplayName = user.DisplayName,
        };
    }

    public UserAdminResponse ToAdminResponse(UserEntity user)
    {
        return new UserAdminResponse
        {
            Id = user.Id,
            Username = user.Username,
            DisplayName = user.DisplayName,
            AdminLevel = (int)user.AdminLevel,
            FacebookId = user.FbId,
            GoogleId = user.GoogleId,
            MicrosoftId = user.MicrosoftId,
            TwitterId = user.TwitterId,
        };
    }

    public UserAccessTokenResponse ToResponse(UserAccessTokenEntity accessToken)
    {
        return new UserAccessTokenResponse
        {
            Id = accessToken.Id,
            Name = accessToken.Name,
            DateCreated = accessToken.DateCreated,
        };
    }

    public UserAccessTokenResponseWithKey ToResponseWithKey(UserAccessTokenEntity accessToken)
    {
        return new UserAccessTokenResponseWithKey
        {
            Id = accessToken.Id,
            Name = accessToken.Name,
            DateCreated = accessToken.DateCreated,
            Key = accessToken.Key,
        };
    }
}
