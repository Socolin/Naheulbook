

// ReSharper disable MemberCanBePrivate.Global

namespace Naheulbook.Core.Features.Event;

public class EventNotFoundException(int eventId) : Exception
{
    public int EventId { get; } = eventId;
}