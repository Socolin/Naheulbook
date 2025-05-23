namespace Naheulbook.Core.Features.Users;

[Serializable]
public class UserAccessTokenNotFoundException(int userId, Guid userAccessTokenId) : Exception("User access token was not found")
{
    public int UserId { get; } = userId;
    public Guid UserAccessTokenId { get; } = userAccessTokenId;
}