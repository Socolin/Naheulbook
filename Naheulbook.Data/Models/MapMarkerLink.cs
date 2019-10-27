// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

namespace Naheulbook.Data.Models
{
    public class MapMarkerLink
    {
        public int Id { get; set; }
        public string? Name { get; set; }

        public int MapMarkerId { get; set; }
        public MapMarker MapMarker { get; set; } = null!;

        public int TargetMapId { get; set; }
        public Map TargetMap { get; set; } = null!;

        public int? TargetMapMarkerId { get; set; }
        public MapMarker? TargetMapMarker { get; set; } = null!;
    }
}