using JetBrains.Annotations;
using Naheulbook.Shared.TransientModels;

namespace Naheulbook.Responses.Responses;

[PublicAPI]
public class CharacterGroupResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public GroupConfig Config { get; set; } = null!;
}