

// ReSharper disable MemberCanBePrivate.Global

namespace Naheulbook.Core.Features.Map;

public class MapMarkerNotFoundException(int mapId) : Exception
{
    public int MapMarkerId { get; } = mapId;
}