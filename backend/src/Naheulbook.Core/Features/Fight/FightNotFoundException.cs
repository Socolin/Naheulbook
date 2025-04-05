

// ReSharper disable MemberCanBePrivate.Global

namespace Naheulbook.Core.Features.Fight;

public class FightNotFoundException(int eventId) : Exception
{
    public int EventId { get; } = eventId;
}