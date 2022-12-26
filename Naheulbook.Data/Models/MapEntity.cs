using System.Collections.Generic;
using Naheulbook.Data.Extensions;

namespace Naheulbook.Data.Models
{
    public class MapEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Data { get; set; } = null!;
        public string ImageData { get; set; } = null!;

        private IEnumerable<MapLayerEntity>? _layers;
        public IEnumerable<MapLayerEntity> Layers { get => _layers.ThrowIfNotLoaded(); set => _layers = value; }
    }
}