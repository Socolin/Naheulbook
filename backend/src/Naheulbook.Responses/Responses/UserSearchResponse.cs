using JetBrains.Annotations;

namespace Naheulbook.Responses.Responses;

[PublicAPI]
public class UserSearchResponse
{
    public int Id { get; set; }
    public string? DisplayName { get; set; }
}