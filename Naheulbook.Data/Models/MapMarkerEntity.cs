using System.Collections.Generic;

namespace Naheulbook.Data.Models
{
    public class MapMarkerEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public string Type { get; set; } = null!;
        public string MarkerInfo { get; set; } = null!;

        public int LayerId { get; set; }
        public MapLayerEntity Layer { get; set; } = null!;

        public ICollection<MapMarkerLinkEntity> Links { get; set; } = null!;
    }
}