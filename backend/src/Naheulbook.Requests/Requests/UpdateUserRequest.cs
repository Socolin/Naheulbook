namespace Naheulbook.Requests.Requests;

[PublicAPI]
public class UpdateUserRequest
{
    public string? DisplayName { get; set; }
    public int? ShowInSearchFor { get; set; }
}