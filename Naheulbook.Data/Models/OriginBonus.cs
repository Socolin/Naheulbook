using System;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

namespace Naheulbook.Data.Models
{
    public class OriginBonus
    {
        public int Id { get; set; }
        public string Description { get; set; } = null!;
        public string? Flags { get; set; }

        public Guid OriginId { get; set; }
        public Origin Origin { get; set; } = null!;
    }
}