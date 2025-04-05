namespace Naheulbook.Shared.TransientModels;

[Serializable]
public class MapImageData
{
    public int ZoomCount { get; set; }
    public uint Width { get; set; }
    public uint Height { get; set; }
    public int ExtraZoomCount { get; set; }
}