using System;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Naheulbook.Core.Exceptions;

public class EventNotFoundException : Exception
{
    public int EventId { get; }

    public EventNotFoundException(int eventId)
    {
        EventId = eventId;
    }
}