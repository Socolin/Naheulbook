using System;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Naheulbook.Core.Features.Map;

public class MapLayerNotFoundException(int mapLayerId) : Exception
{
    public int MapLayerId { get; } = mapLayerId;
}