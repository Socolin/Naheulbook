namespace Naheulbook.Requests.Requests;

[PublicAPI]
public class PostCreateGroupHistoryEntryRequest
{
    public bool IsGm { get; set; }
    public required string Info { get; set; }
}