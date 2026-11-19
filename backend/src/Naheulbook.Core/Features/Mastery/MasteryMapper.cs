using Naheulbook.Core.Features.Shared;
using Naheulbook.Data.EntityFrameworkCore.Entities;
using Naheulbook.Responses.Responses;
using Naheulbook.Shared.TransientModels;

namespace Naheulbook.Core.Features.Mastery;

public interface IMasteryMapper
{
    MasteryResponse ToResponse(MasteryEntity mastery);
    MasteryGroupResponse ToResponse(MasteryGroupEntity masteryGroup);
}

public class MasteryMapper : IMasteryMapper
{
    public MasteryResponse ToResponse(MasteryEntity mastery)
    {
        return new MasteryResponse
        {
            Id = mastery.Id,
            Name = mastery.Name,
            Data = MapperHelpers.FromJsonNotNull<MasteryData>(mastery.Data),
        };
    }

    public MasteryGroupResponse ToResponse(MasteryGroupEntity masteryGroup)
    {
        return new MasteryGroupResponse
        {
            Id = masteryGroup.Id,
            Name = masteryGroup.Name,
            Description = masteryGroup.Description,
            Masteries = masteryGroup.Masteries
                .OrderBy(x => x.DisplayOrder)
                .Select(ToResponse)
                .ToList(),
        };
    }
}
