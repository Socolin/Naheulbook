using System;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

namespace Naheulbook.Data.Models
{
    public class JobRestriction
    {
        public int Id { get; set; }
        public string Text { get; set; } = null!;
        public string? Flags { get; set; }

        public Guid JobId { get; set; }
        public Job Job { get; set; } = null!;
    }
}