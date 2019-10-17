namespace Naheulbook.Core.Configurations
{
    public class MapImageConfiguration
    {
        public int TilesSize { get; set; } = 256;
        public int MinZoomMapSize { get; set; } = 512;
        public string OutputDirectory { get; set; }
        public int ExtraZoomCount { get; set; } = 1;
    }
}