using System.Collections.Generic;
using Naheulbook.Data.Models;

namespace Naheulbook.Core.Models;

public class LevelUpResult
{
    public IList<CharacterModifierEntity> NewModifiers { get; set; } = new List<CharacterModifierEntity>();
    public int NewLevel { get; set; }
    public IList<CharacterSkillEntity> NewSkills { get; set; } = new List<CharacterSkillEntity>();
    public IList<CharacterSpecialityEntity> NewSpecialities { get; set; } = new List<CharacterSpecialityEntity>();
}