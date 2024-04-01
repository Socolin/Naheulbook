using System;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Naheulbook.Core.Exceptions;

public class LocationNotFoundException(int locationId) : Exception
{
    public int LocationId { get; } = locationId;
}