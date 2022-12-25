namespace Naheulbook.Data.Models
{
    public class MapMarkerLinkEntity
    {
        public int Id { get; set; }
        public string? Name { get; set; }

        public int MapMarkerId { get; set; }
        public MapMarkerEntity MapMarker { get; set; } = null!;

        public int TargetMapId { get; set; }
        public MapEntity TargetMap { get; set; } = null!;

        public int? TargetMapMarkerId { get; set; }
        public MapMarkerEntity? TargetMapMarker { get; set; } = null!;
    }
}