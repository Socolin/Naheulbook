// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

namespace Naheulbook.Core.Features.Map;

public class MapImageConfiguration
{
    public uint TilesSize { get; set; } = 256;
    public int MinZoomMapSize { get; set; } = 512;
    public string OutputDirectory { get; set; } = null!;
    public int ExtraZoomCount { get; set; } = 1;
}