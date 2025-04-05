using JetBrains.Annotations;

namespace Naheulbook.Web.Responses;

[PublicAPI]
public class DeleteInviteResponse
{
    public int GroupId { get; set; }
    public int CharacterId { get; set; }
    public bool FromGroup { get; set; }
}