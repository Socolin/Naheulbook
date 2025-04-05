namespace Naheulbook.Requests.Requests;

[PublicAPI]
public class CreateMapRequest
{
    [PublicAPI]
    public class CreateMapRequestData
    {
        [PublicAPI]
        public class MapAttributionRequest
        {
            public required string Name { get; set; }
            public required string Url { get; set; }
        }

        public bool IsGm { get; set; }
        public string UnitName { get; set; } = "m";
        public double PixelPerUnit { get; set; } = 5;
        public required List<MapAttributionRequest> Attribution { get; set; } = new();
    }

    public required string Name { get; set; }
    public required CreateMapRequestData Data { get; set; }
}