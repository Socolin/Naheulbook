using System;

namespace Naheulbook.Data.Models
{
    public class CharacterSkillEntity
    {
        public int CharacterId { get; set; }
        public CharacterEntity Character { get; set; } = null!;

        public Guid SkillId { get; set; }
        public SkillEntity Skill { get; set; } = null!;
    }
}