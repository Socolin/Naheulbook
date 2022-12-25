using System.Collections.Generic;

namespace Naheulbook.Data.Models
{
    public class MapEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Data { get; set; } = null!;
        public string ImageData { get; set; } = null!;

        public IEnumerable<MapLayerEntity> Layers { get; set; } = null!;
    }
}