using System.Collections.Generic;

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

            public List<MapAttribution> Attribution { get; set; } = new List<MapAttribution>();
        }

        public string Name { get; set; } = null!;
        public CreateMapRequestData Data { get; set; } = null!;
    }
}