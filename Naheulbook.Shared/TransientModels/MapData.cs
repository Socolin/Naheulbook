using System.Collections.Generic;

namespace Naheulbook.Shared.TransientModels
{
    public class MapData
    {
        public class MapAttribution
        {
            public string Name { get; set; }
            public string Url { get; set; }
        }

        public string UnitName { get; set; } = "m";
        public double PixelPerUnit { get; set; } = 5;
        public List<MapAttribution> Attribution { get; set; } = new List<MapAttribution>();
    }
}