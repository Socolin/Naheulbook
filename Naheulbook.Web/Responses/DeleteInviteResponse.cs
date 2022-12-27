namespace Naheulbook.Web.Responses;

public class DeleteInviteResponse
{
    public int GroupId { get; set; }
    public int CharacterId { get; set; }
    public bool FromGroup { get; set; }
}