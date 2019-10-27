using System.Collections.Generic;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

namespace Naheulbook.Data.Models
{
    public class MapLayer
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Source { get; set; } = null!;

        public int MapId { get; set; }
        public Map Map { get; set; } = null!;

        public int? UserId { get; set; }
        public User? User { get; set; }

        public ICollection<MapMarker> Markers { get; set; } = null!;
    }
}