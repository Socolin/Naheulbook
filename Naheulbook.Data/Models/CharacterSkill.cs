using System;

// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

namespace Naheulbook.Data.Models
{
    public class CharacterSkill
    {
        public int CharacterId { get; set; }
        public Character Character { get; set; } = null!;

        public Guid SkillId { get; set; }
        public Skill Skill { get; set; } = null!;
    }
}