

// ReSharper disable MemberCanBePrivate.Global

namespace Naheulbook.Core.Features.Map;

public class MapMarkerLinkNotFoundException(int mapMarkerLinkId) : Exception
{
    public int MapMarkerLinkId { get; } = mapMarkerLinkId;
}