using System;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Naheulbook.Core.Exceptions;

public class LocationNotFoundException : Exception
{
    public int LocationId { get; }

    public LocationNotFoundException(int locationId)
    {
        LocationId = locationId;
    }
}