using Naheulbook.Data.EntityFrameworkCore.Extensions;

namespace Naheulbook.Data.EntityFrameworkCore.Entities;

[Serializable]
public class SkillEffectEntity
{
    public string StatName { get; set; } = null!;
    public int Value { get; set; }

    public Guid SkillId { get; set; }
    private SkillEntity? _skill;
    public SkillEntity Skill { get => _skill.ThrowIfNotLoaded(); set => _skill = value; }
}