using System;
using System.Collections.Generic;

// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Naheulbook.Data.Models
{
    public class Job
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Information { get; set; }
        public string? PlayerDescription { get; set; }
        public string? PlayerSummary { get; set; }
        public string? Flags { get; set; }
        public string Data { get; set; } = null!;
        public bool? IsMagic { get; set; }

        public ICollection<JobBonus> Bonuses { get; set; } = null!;
        public ICollection<JobRequirement> Requirements { get; set; } = null!;
        public ICollection<JobRestriction> Restrictions { get; set; } = null!;
        public ICollection<JobSkill> Skills { get; set; } = null!;
        public ICollection<Speciality> Specialities { get; set; } = null!;
    }
}