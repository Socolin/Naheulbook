using System;
using System.Collections.Generic;
using Naheulbook.Data.Extensions;

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

        private ICollection<JobBonusEntity>? _bonuses;
        public ICollection<JobBonusEntity> Bonuses { get => _bonuses.ThrowIfNotLoaded(); set => _bonuses = value; }

        private ICollection<JobRequirementEntity>? _requirements;
        public ICollection<JobRequirementEntity> Requirements { get => _requirements.ThrowIfNotLoaded(); set => _requirements = value; }

        private ICollection<JobRestrictionEntity>? _restrictions;
        public ICollection<JobRestrictionEntity> Restrictions { get => _restrictions.ThrowIfNotLoaded(); set => _restrictions = value; }

        private ICollection<JobSkillEntity> _skills = null!;
        public ICollection<JobSkillEntity> Skills { get => _skills.ThrowIfNotLoaded(); set => _skills = value; }

        private ICollection<SpecialityEntity>? _specialities;
        public ICollection<SpecialityEntity> Specialities { get => _specialities.ThrowIfNotLoaded(); set => _specialities = value; }
    }
}