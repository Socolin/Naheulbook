using System;

namespace Naheulbook.Data.Models
{
    public class OriginInfoEntity
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;

        public Guid OriginId { get; set; }
        public OriginEntity Origin { get; set; } = null!;
    }
}