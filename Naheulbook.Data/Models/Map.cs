using System.Collections.Generic;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

namespace Naheulbook.Data.Models
{
    public class Map
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Data { get; set; } = null!;
        public string ImageData { get; set; } = null!;

        public IEnumerable<MapLayer> Layers { get; set; } = null!;
    }
}