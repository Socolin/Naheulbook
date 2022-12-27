using System;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Naheulbook.Core.Exceptions;

public class MapNotFoundException : Exception
{
    public int MapId { get; }

    public MapNotFoundException(int mapId)
    {
        MapId = mapId;
    }
}