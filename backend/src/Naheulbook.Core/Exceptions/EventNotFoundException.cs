using System;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Naheulbook.Core.Exceptions;

public class EventNotFoundException(int eventId) : Exception
{
    public int EventId { get; } = eventId;
}