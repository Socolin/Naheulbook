using System;
using Naheulbook.Data.Extensions;

namespace Naheulbook.Data.Models
{
    public class SkillEffect
    {
        public string StatName { get; set; } = null!;
        public int Value { get; set; }

        public Guid SkillId { get; set; }
        private SkillEntity? _skill;
        public SkillEntity Skill { get => _skill.ThrowIfNotLoaded(); set => _skill = value; }
    }
}