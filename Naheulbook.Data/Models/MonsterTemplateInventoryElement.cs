using System;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

namespace Naheulbook.Data.Models
{
    public class MonsterTemplateInventoryElement
    {
        public int Id { get; set; }
        public float Chance { get; set; }
        public int MinCount { get; set; }
        public int MaxCount { get; set; }

        public Guid ItemTemplateId { get; set; }
        public ItemTemplate ItemTemplate { get; set; } = null!;

        public int MonsterTemplateId { get; set; }
        public MonsterTemplate MonsterTemplate { get; set; } = null!;
    }
}