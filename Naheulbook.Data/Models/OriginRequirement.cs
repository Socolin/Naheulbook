using System;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

namespace Naheulbook.Data.Models
{
    public class OriginRequirement
    {
        public int Id { get; set; }

        public int? MinValue { get; set; }
        public int? MaxValue { get; set; }

        public Guid OriginId { get; set; }
        public Origin Origin { get; set; } = null!;

        public string StatName { get; set; } = null!;
        public Stat Stat { get; set; } = null!;
    }
}