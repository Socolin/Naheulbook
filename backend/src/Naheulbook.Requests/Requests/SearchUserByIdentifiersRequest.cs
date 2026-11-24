namespace Naheulbook.Requests.Requests;

[PublicAPI]
public class SearchUserByIdentifiersRequest
{
    public string? Username { get; set; }
    public string? FacebookId { get; set; }
    public string? GoogleId { get; set; }
    public string? MicrosoftId { get; set; }
    public string? TwitterId { get; set; }
}
