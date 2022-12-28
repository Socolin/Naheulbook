namespace Naheulbook.Requests.Requests;

[PublicAPI]
public class CreateInviteRequest
{
    public int CharacterId { get; set; }
    public bool FromGroup { get; set; }
}