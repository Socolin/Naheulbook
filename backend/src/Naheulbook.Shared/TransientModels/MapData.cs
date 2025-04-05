namespace Naheulbook.Shared.TransientModels;

[Serializable]
public class MapData
{
    [Serializable]
    public class MapAttribution
    {
        public string Name { get; set; } = null!;
        public string Url { get; set; } = null!;
    }

    public string UnitName { get; set; } = "m";
    public double PixelPerUnit { get; set; } = 5;
    public bool IsGm { get; set; }
    public List<MapAttribution> Attribution { get; set; } = new();
}