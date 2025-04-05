using JetBrains.Annotations;

namespace Naheulbook.Web.Responses;

[PublicAPI]
public class UserAccessTokenResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public DateTimeOffset DateCreated { get; set; }
}

[PublicAPI]
public class UserAccessTokenResponseWithKey : UserAccessTokenResponse
{
    public string Key { get; set; } = null!;
}