using JetBrains.Annotations;
using Naheulbook.Shared.TransientModels;

namespace Naheulbook.Responses.Responses;

[PublicAPI]
public class TalentResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public TalentData Data { get; set; } = null!;
}
