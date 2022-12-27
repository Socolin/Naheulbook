using System;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Naheulbook.Core.Exceptions;

public class MapMarkerLinkNotFoundException : Exception
{
    public int MapMarkerLinkId { get; }

    public MapMarkerLinkNotFoundException(int mapMarkerLinkId)
    {
        MapMarkerLinkId = mapMarkerLinkId;
    }
}