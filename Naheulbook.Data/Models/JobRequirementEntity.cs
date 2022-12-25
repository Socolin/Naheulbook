using System;

namespace Naheulbook.Data.Models
{
    public class JobRequirementEntity
    {
        public int Id { get; set; }

        public long? MinValue { get; set; }
        public long? MaxValue { get; set; }

        public string StatName { get; set; } = null!;
        public StatEntity Stat { get; set; } = null!;

        public Guid JobId { get; set; }
        public JobEntity Job { get; set; } = null!;
    }
}