using Naheulbook.Core.Features.Shared;
using Naheulbook.Data.EntityFrameworkCore.Entities;
using Naheulbook.Responses.Responses;
using Naheulbook.Shared.TransientModels;

namespace Naheulbook.Core.Features.Origin;

public interface IOriginMapper
{
    OriginResponse ToResponse(OriginEntity origin);
    OriginInformationResponse ToResponse(OriginInfoEntity originInfo);
    OriginRequirementResponse ToResponse(OriginRequirementEntity originRequirement);
    DescribedFlagResponse ToResponse(OriginBonusEntity originBonus);
    DescribedFlagResponse ToResponse(OriginRestrictEntity originRestrict);
}

public class OriginMapper : IOriginMapper
{
    public OriginResponse ToResponse(OriginEntity origin)
    {
        return new OriginResponse
        {
            Id = origin.Id,
            Name = origin.Name,
            Description = origin.Description,
            PlayerDescription = origin.PlayerDescription,
            PlayerSummary = origin.PlayerSummary,
            Advantage = origin.Advantage,
            Data = MapperHelpers.FromJsonNotNull<OriginData>(origin.Data),
            Size = origin.Size,
            Flags = MapperHelpers.FromJson<List<FlagResponse>>(origin.Flags) ?? [],
            SkillIds = origin.Skills.Where(s => s.Default).OrderBy(s => s.SkillId).Select(s => s.SkillId).ToList(),
            AvailableSkillIds = origin.Skills.Where(s => !s.Default).OrderBy(s => s.SkillId).Select(s => s.SkillId).ToList(),
            AptitudeGroupId = origin.AptitudeGroupId,
            Information = origin.Information.Select(ToResponse).ToList(),
            Bonuses = origin.Bonuses.Select(ToResponse).ToList(),
            Requirements = origin.Requirements.OrderBy(r => r.Id).Select(ToResponse).ToList(),
            Restrictions = origin.Restrictions.Select(ToResponse).ToList(),
        };
    }

    public OriginInformationResponse ToResponse(OriginInfoEntity originInfo)
    {
        return new OriginInformationResponse
        {
            Title = originInfo.Title,
            Description = originInfo.Description,
        };
    }

    public OriginRequirementResponse ToResponse(OriginRequirementEntity originRequirement)
    {
        return new OriginRequirementResponse
        {
            Stat = originRequirement.StatName,
            Min = originRequirement.MinValue,
            Max = originRequirement.MaxValue,
        };
    }

    public DescribedFlagResponse ToResponse(OriginBonusEntity originBonus)
    {
        return new DescribedFlagResponse
        {
            Description = originBonus.Description,
            Flags = MapperHelpers.FromJson<List<FlagResponse>>(originBonus.Flags) ?? [],
        };
    }

    public DescribedFlagResponse ToResponse(OriginRestrictEntity originRestrict)
    {
        return new DescribedFlagResponse
        {
            Description = originRestrict.Text,
            Flags = MapperHelpers.FromJson<List<FlagResponse>>(originRestrict.Flags) ?? [],
        };
    }
}
