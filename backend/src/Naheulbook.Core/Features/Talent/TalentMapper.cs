using Naheulbook.Core.Features.Shared;
using Naheulbook.Data.EntityFrameworkCore.Entities;
using Naheulbook.Responses.Responses;
using Naheulbook.Shared.TransientModels;

namespace Naheulbook.Core.Features.Talent;

public interface ITalentMapper
{
    TalentResponse ToResponse(TalentEntity talent);
    TalentGroupResponse ToResponse(TalentGroupEntity talentGroup);
}

public class TalentMapper : ITalentMapper
{
    public TalentResponse ToResponse(TalentEntity talent)
    {
        return new TalentResponse
        {
            Id = talent.Id,
            Name = talent.Name,
            Data = MapperHelpers.FromJsonNotNull<TalentData>(talent.Data),
        };
    }

    public TalentGroupResponse ToResponse(TalentGroupEntity talentGroup)
    {
        return new TalentGroupResponse
        {
            Id = talentGroup.Id,
            Name = talentGroup.Name,
            Description = talentGroup.Description,
            Talents = talentGroup.Talents
                .OrderBy(x => x.DisplayOrder)
                .Select(ToResponse)
                .ToList(),
        };
    }
}
