namespace Naheulbook.Core.Features.Entropy;

public class EntropyEventNotFoundException(EntropyKind kind, Guid eventId) : Exception($"No {kind} found with id {eventId}")
{
    public EntropyKind Kind { get; } = kind;
    public Guid EventId { get; } = eventId;
}
