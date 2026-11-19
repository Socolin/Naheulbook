using JetBrains.Annotations;
using Naheulbook.Shared.TransientModels;

namespace Naheulbook.Responses.Responses;

[PublicAPI]
public class HandicapsResponse
{
    public IReadOnlyCollection<HandicapResponse> Handicaps { get; set; } = [];
}

[PublicAPI]
public class HandicapResponse
{
    public Guid Id { get; set; }
    public int DiceRoll { get; set; }
    public string Name { get; set; } = null!;
    public HandicapData Data { get; set; } = null!;
}
