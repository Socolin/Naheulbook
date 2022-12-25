using System;

namespace Naheulbook.Data.Models
{
    public class OriginRandomNameUrlEntity
    {
        public int Id { get; set; }

        public string Sex { get; set; } = null!;
        public string Url { get; set; } = null!;

        public Guid OriginId { get; set; }
        public OriginEntity Origin { get; set; } = null!;
    }
}