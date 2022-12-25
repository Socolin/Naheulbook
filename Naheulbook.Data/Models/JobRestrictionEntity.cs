using System;

namespace Naheulbook.Data.Models
{
    public class JobRestrictionEntity
    {
        public int Id { get; set; }
        public string Text { get; set; } = null!;
        public string? Flags { get; set; }

        public Guid JobId { get; set; }
        public JobEntity Job { get; set; } = null!;
    }
}