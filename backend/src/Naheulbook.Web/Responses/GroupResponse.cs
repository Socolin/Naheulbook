using JetBrains.Annotations;
using Naheulbook.Shared.TransientModels;
using Newtonsoft.Json.Linq;

namespace Naheulbook.Web.Responses;

[PublicAPI]
public class GroupResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public JObject? Data { get; set; }
    public GroupConfig Config { get; set; } = null!;

    public IList<int> CharacterIds { get; set; } = null!;
    public IList<GroupGroupInviteResponse> Invites { get; set; } = null!;
}