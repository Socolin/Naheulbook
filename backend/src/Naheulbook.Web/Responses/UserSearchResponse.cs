using JetBrains.Annotations;

namespace Naheulbook.Web.Responses;

[PublicAPI]
public class UserSearchResponse
{
    public int Id { get; set; }
    public string? DisplayName { get; set; }
}