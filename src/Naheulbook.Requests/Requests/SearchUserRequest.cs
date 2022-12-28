namespace Naheulbook.Requests.Requests;

[PublicAPI]
public class SearchUserRequest
{
    public required string Filter { get; set; }
}