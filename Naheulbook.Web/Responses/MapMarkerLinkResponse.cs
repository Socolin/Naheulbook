// ReSharper disable UnusedMember.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

namespace Naheulbook.Web.Responses
{
    public class MapMarkerLinkResponse
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string TargetMapName { get; set; } = null!;
        public int TargetMapId { get; set; }
        public int? TargetMapMarkerId { get; set; }
    }
}