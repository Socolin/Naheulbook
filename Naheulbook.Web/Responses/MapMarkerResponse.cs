using System.Collections.Generic;
using Newtonsoft.Json.Linq;

// ReSharper disable UnusedMember.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

namespace Naheulbook.Web.Responses
{
    public class MapMarkerResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public string Type { get; set; } = null!;
        public JObject MarkerInfo { get; set; } = null!;
        public List<MapMarkerLinkResponse> Links { get; set; } = null!;
    }
}