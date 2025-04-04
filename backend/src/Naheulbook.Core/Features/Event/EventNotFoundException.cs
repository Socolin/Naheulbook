using System;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Naheulbook.Core.Features.Event;

public class EventNotFoundException(int eventId) : Exception
{
    public int EventId { get; } = eventId;
}