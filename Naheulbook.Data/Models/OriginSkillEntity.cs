using System;

namespace Naheulbook.Data.Models
{
    public class OriginSkillEntity
    {
        public bool Default { get; set; }

        public Guid OriginId { get; set; }
        public OriginEntity Origin { get; set; } = null!;

        public Guid SkillId { get; set; }
        public SkillEntity Skill { get; set; } = null!;
    }
}