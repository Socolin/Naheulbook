using Naheulbook.Data.EntityFrameworkCore.Entities;

namespace Naheulbook.Core.Features.Character;

[Serializable]
public class LevelUpResult
{
    public IList<CharacterModifierEntity> NewModifiers { get; set; } = new List<CharacterModifierEntity>();
    public int NewLevel { get; set; }
    public IList<CharacterSkillEntity> NewSkills { get; set; } = new List<CharacterSkillEntity>();
    public IList<CharacterSpecialityEntity> NewSpecialities { get; set; } = new List<CharacterSpecialityEntity>();
}