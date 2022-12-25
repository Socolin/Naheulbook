using System;

namespace Naheulbook.Data.Models
{
    public class OriginRequirementEntity
    {
        public int Id { get; set; }

        public int? MinValue { get; set; }
        public int? MaxValue { get; set; }

        public Guid OriginId { get; set; }
        public OriginEntity Origin { get; set; } = null!;

        public string StatName { get; set; } = null!;
        public StatEntity Stat { get; set; } = null!;
    }
}