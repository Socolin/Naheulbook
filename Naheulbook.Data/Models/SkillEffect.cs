using System;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

namespace Naheulbook.Data.Models
{
    public class SkillEffect
    {
        public string StatName { get; set; } = null!;
        public int Value { get; set; }

        public Guid SkillId { get; set; }
        public SkillEntity Skill { get; set; } = null!;
    }
}