using System;
using System.Collections.Generic;
using Naheulbook.Data.Extensions;

namespace Naheulbook.Data.Models
{
    public class OriginEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string Data { get; set; } = null!;
        public string? PlayerDescription { get; set; }
        public string? PlayerSummary { get; set; }
        public string? Advantage { get; set; }
        public string? Size { get; set; }
        public string? Flags { get; set; }

        private ICollection<OriginBonus>? _bonuses;
        public ICollection<OriginBonus> Bonuses { get => _bonuses.ThrowIfNotLoaded(); set => _bonuses = value; }

        private ICollection<OriginInfoEntity>? _information;
        public ICollection<OriginInfoEntity> Information { get => _information.ThrowIfNotLoaded(); set => _information = value; }

        private ICollection<OriginRequirementEntity>? _requirements;
        public ICollection<OriginRequirementEntity> Requirements { get => _requirements.ThrowIfNotLoaded(); set => _requirements = value; }

        private ICollection<OriginRestrictEntity>? _restrictions;
        public ICollection<OriginRestrictEntity> Restrictions { get => _restrictions.ThrowIfNotLoaded(); set => _restrictions = value; }

        private ICollection<OriginSkillEntity>? _skills;
        public ICollection<OriginSkillEntity> Skills { get => _skills.ThrowIfNotLoaded(); set => _skills = value; }
    }
}