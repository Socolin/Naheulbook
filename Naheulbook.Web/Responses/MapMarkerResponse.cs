// ReSharper disable UnusedMember.Global

using Newtonsoft.Json.Linq;

namespace Naheulbook.Web.Responses
{
    public class MapMarkerResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public string Type { get; set; } = null!;
        public JObject MarkerInfo { get; set; } = null!;
    }
}