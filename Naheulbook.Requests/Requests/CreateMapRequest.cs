using System.Collections.Generic;

// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable CollectionNeverUpdated.Global

namespace Naheulbook.Requests.Requests
{
    public class CreateMapRequest
    {
        public class CreateMapRequestData
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

        public string Name { get; set; } = null!;
        public CreateMapRequestData Data { get; set; } = null!;
    }
}