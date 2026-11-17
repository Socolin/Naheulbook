using Naheulbook.Core.Features.Shared;
using Naheulbook.Data.EntityFrameworkCore.Entities;
using Naheulbook.Responses.Responses;

namespace Naheulbook.Core.Features.Skill;

public interface ISkillMapper
{
    SkillResponse ToResponse(SkillEntity skill);
    SkillEffectResponse ToResponse(SkillEffectEntity skillEffect);
}

public class SkillMapper : ISkillMapper
{
    public SkillResponse ToResponse(SkillEntity skill)
    {
        return new SkillResponse
        {
            Id = skill.Id,
            Name = skill.Name,
            Description = skill.Description,
            PlayerDescription = skill.PlayerDescription,
            Require = skill.Require,
            Resist = skill.Resist,
            Using = skill.Using,
            Roleplay = skill.Roleplay,
            Stat = MapperHelpers.FromCommaSeparatedStringArray(skill.Stat),
            Test = skill.Test,
            Flags = MapperHelpers.FromJson<List<FlagResponse>>(skill.Flags) ?? [],
            Effects = skill.SkillEffects.Select(ToResponse).ToList(),
        };
    }

    public SkillEffectResponse ToResponse(SkillEffectEntity skillEffect)
    {
        return new SkillEffectResponse
        {
            Stat = skillEffect.StatName,
            Value = skillEffect.Value,
            Type = "ADD",
        };
    }
}
