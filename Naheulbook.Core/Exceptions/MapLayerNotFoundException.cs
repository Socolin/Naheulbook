using System;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Naheulbook.Core.Exceptions;

public class MapLayerNotFoundException : Exception
{
    public int MapLayerId { get; }

    public MapLayerNotFoundException(int mapLayerId)
    {
        MapLayerId = mapLayerId;
    }
}