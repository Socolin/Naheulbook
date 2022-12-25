using System;
using System.Collections.Generic;

namespace Naheulbook.Data.Models
{
    public class JobEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Information { get; set; }
        public string PlayerDescription { get; set; } = null!;
        public string PlayerSummary { get; set; } = null!;
        public string? Flags { get; set; }
        public string Data { get; set; } = null!;
        public bool? IsMagic { get; set; }

        public ICollection<JobBonus> Bonuses { get; set; } = null!;
        public ICollection<JobRequirementEntity> Requirements { get; set; } = null!;
        public ICollection<JobRestrictionEntity> Restrictions { get; set; } = null!;
        public ICollection<JobSkill> Skills { get; set; } = null!;
        public ICollection<SpecialityEntity> Specialities { get; set; } = null!;
    }
}