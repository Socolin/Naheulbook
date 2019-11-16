using System;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

namespace Naheulbook.Data.Models
{
    public class JobSkill
    {
        public bool Default { get; set; }

        public int JobId { get; set; }
        public Job Job { get; set; } = null!;

        public Guid SkillId { get; set; }
        public Skill Skill { get; set; } = null!;
    }
}