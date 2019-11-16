using System;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

namespace Naheulbook.Data.Models
{
    public class OriginRestrict
    {
        public int Id { get; set; }
        public string Text { get; set; } = null!;
        public string? Flags { get; set; }

        public Guid OriginId { get; set; }
        public Origin Origin { get; set; } = null!;
    }
}

