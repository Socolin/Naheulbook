using System;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Naheulbook.Core.Exceptions;

public class MapMarkerLinkNotFoundException(int mapMarkerLinkId) : Exception
{
    public int MapMarkerLinkId { get; } = mapMarkerLinkId;
}