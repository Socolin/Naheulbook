using Newtonsoft.Json.Linq;

namespace Naheulbook.Requests.Requests
{
    public class CreateMapMarkerRequest
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public string Type { get; set; } = null!;
        public JObject MarkerInfo { get; set; } = null!;
    }
}