using System;

namespace Naheulbook.Data.Models
{
    public class OriginRestrictEntity
    {
        public int Id { get; set; }
        public string Text { get; set; } = null!;
        public string? Flags { get; set; }

        public Guid OriginId { get; set; }
        public OriginEntity Origin { get; set; } = null!;
    }
}

