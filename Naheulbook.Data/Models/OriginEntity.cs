using System;
using System.Collections.Generic;

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

        public ICollection<OriginBonus> Bonuses { get; set; } = null!;
        public ICollection<OriginInfoEntity> Information { get; set; } = null!;
        public ICollection<OriginRequirementEntity> Requirements { get; set; } = null!;
        public ICollection<OriginRestrictEntity> Restrictions { get; set; } = null!;
        public ICollection<OriginSkillEntity> Skills { get; set; } = null!;
    }
}