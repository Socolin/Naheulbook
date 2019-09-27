using System.Collections.Generic;

namespace Naheulbook.Data.Models
{
    public class Location
    {
        public int Id { get; set; }
        public string? Data { get; set; }
        public string Name { get; set; } = null!;

        public int? ParentId { get; set; }
        public Location Parent { get; set; } = null!;

        public ICollection<LocationMap> Maps { get; set; } = null!;
    }
}