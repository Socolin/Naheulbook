using System.Collections.Generic;

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Naheulbook.Shared.TransientModels
{
    public class MapData
    {
        public class MapAttribution
        {
            public string Name { get; set; } = null!;
            public string Url { get; set; } = null!;
        }

        public string UnitName { get; set; } = "m";
        public double PixelPerUnit { get; set; } = 5;
        public bool IsGm { get; set; }
        public List<MapAttribution> Attribution { get; set; } = new List<MapAttribution>();
    }
}