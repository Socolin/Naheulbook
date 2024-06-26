using System;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Naheulbook.Core.Exceptions;

public class MapMarkerNotFoundException(int mapId) : Exception
{
    public int MapMarkerId { get; } = mapId;
}