using System;
using System.Collections.Generic;

// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Naheulbook.Data.Models
{
    public class Skill
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public string? PlayerDescription { get; set; }
        public string? Require { get; set; }
        public string? Resist { get; set; }
        public string? Using { get; set; }
        public string? Roleplay { get; set; }
        public string? Stat { get; set; }
        public short? Test { get; set; }
        public string? Flags { get; set; }

        public ICollection<SkillEffect> SkillEffects { get; set; } = null!;

        public ICollection<OriginSkill> OriginSkills { get; set; } = null!;
        public ICollection<JobSkill> JobSkills { get; set; } = null!;
    }
}