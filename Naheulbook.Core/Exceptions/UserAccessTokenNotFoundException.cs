using System;

namespace Naheulbook.Core.Exceptions;

public class UserAccessTokenNotFoundException : Exception
{
    public int UserId { get; }
    public Guid UserAccessTokenId { get; }

    public UserAccessTokenNotFoundException(int userId, Guid userAccessTokenId)
        : base("User access token was not found")
    {
        UserId = userId;
        UserAccessTokenId = userAccessTokenId;
    }
}