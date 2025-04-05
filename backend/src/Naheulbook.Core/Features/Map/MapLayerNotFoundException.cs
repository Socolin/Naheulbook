

// ReSharper disable MemberCanBePrivate.Global

namespace Naheulbook.Core.Features.Map;

public class MapLayerNotFoundException(int mapLayerId) : Exception
{
    public int MapLayerId { get; } = mapLayerId;
}