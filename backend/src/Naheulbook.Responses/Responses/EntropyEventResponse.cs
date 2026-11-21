using JetBrains.Annotations;

namespace Naheulbook.Responses.Responses;

[PublicAPI]
public class EntropyTablesResponse
{
    public IReadOnlyCollection<EntropyEventResponse> MankdebolCalamities { get; set; } = [];
    public IReadOnlyCollection<EntropyEventResponse> DebilibeukBlessings { get; set; } = [];
}

[PublicAPI]
public class EntropyEventResponse
{
    public Guid Id { get; set; }
    public int DiceRoll { get; set; }
    public string Name { get; set; } = null!;
    public string Effect { get; set; } = null!;
}
