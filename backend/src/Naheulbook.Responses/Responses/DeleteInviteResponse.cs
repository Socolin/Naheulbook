using JetBrains.Annotations;

namespace Naheulbook.Responses.Responses;

[PublicAPI]
public class DeleteInviteResponse
{
    public int GroupId { get; set; }
    public int CharacterId { get; set; }
    public bool FromGroup { get; set; }
}