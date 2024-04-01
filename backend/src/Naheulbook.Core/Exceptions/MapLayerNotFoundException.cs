using System;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Naheulbook.Core.Exceptions;

public class MapLayerNotFoundException(int mapLayerId) : Exception
{
    public int MapLayerId { get; } = mapLayerId;
}