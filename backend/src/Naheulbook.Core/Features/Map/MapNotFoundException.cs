

// ReSharper disable MemberCanBePrivate.Global

namespace Naheulbook.Core.Features.Map;

public class MapNotFoundException(int mapId) : Exception
{
    public int MapId { get; } = mapId;
}