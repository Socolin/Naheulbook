using System.Collections.Generic;
using Naheulbook.Shared.TransientModels;

namespace Naheulbook.Requests.Requests;

[PublicAPI]
public class PostGroupUpdateDurationsRequest
{
    public int? MonsterId { get; set; }
    public int? CharacterId { get; set; }
    public required IList<DurationChangeRequest> Changes { get; set; }
}

[PublicAPI]
public class DurationChangeRequest
{
    public required string Type { get; set; }
    public int? ItemId { get; set; }
    public NewModifierDurationValue? Modifier { get; set; }
    public LifeTime? LifeTime { get; set; }
}