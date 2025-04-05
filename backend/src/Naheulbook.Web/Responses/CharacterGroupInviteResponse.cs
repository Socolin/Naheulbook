using JetBrains.Annotations;
using Naheulbook.Shared.TransientModels;

namespace Naheulbook.Web.Responses;

[PublicAPI]
public class CharacterGroupInviteResponse
{
    public string GroupName { get; set; } = null!;
    public int GroupId { get; set; }
    public GroupConfig Config { get; set; } = null!;
}