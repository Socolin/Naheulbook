using System.Collections.Generic;
using Naheulbook.Data.Models;

namespace Naheulbook.Core.Models
{
    public class LevelUpResult
    {
        public IList<CharacterModifier> NewModifiers { get; set; } = new List<CharacterModifier>();
        public int NewLevel { get; set; }
        public IList<CharacterSkill> NewSkills { get; set; } = new List<CharacterSkill>();
        public IList<CharacterSpeciality> NewSpecialities { get; set; } = new List<CharacterSpeciality>();
    }
}