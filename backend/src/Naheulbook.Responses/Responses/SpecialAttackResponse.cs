using JetBrains.Annotations;
using Naheulbook.Shared.TransientModels;

namespace Naheulbook.Responses.Responses;

[PublicAPI]
public class SpecialAttackResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public SpecialAttackData Data { get; set; } = null!;
}
