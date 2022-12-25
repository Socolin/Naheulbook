using System.Collections.Generic;

namespace Naheulbook.Data.Models
{
    public class MapLayerEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Source { get; set; } = null!;
        public bool IsGm { get; set; }

        public int MapId { get; set; }
        public MapEntity Map { get; set; } = null!;

        public int? UserId { get; set; }
        public UserEntity? User { get; set; }

        public ICollection<MapMarkerEntity> Markers { get; set; } = null!;
    }
}