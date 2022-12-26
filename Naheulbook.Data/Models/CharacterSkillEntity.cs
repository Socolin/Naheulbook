using System;
using Naheulbook.Data.Extensions;

namespace Naheulbook.Data.Models
{
    public class CharacterSkillEntity
    {
        public int CharacterId { get; set; }
        private CharacterEntity? _character;
        public CharacterEntity Character { get => _character.ThrowIfNotLoaded(); set => _character = value; }

        public Guid SkillId { get; set; }
        private SkillEntity? _skill;
        public SkillEntity Skill { get => _skill.ThrowIfNotLoaded(); set => _skill = value; }
    }
}