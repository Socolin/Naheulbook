using Naheulbook.Core.Features.Shared;
using Naheulbook.Data.EntityFrameworkCore.Entities;
using Naheulbook.Responses.Responses;
using Naheulbook.Shared.TransientModels;

namespace Naheulbook.Core.Features.Handicap;

public interface IHandicapMapper
{
    HandicapResponse ToResponse(HandicapEntity handicap);
}

public class HandicapMapper : IHandicapMapper
{
    public HandicapResponse ToResponse(HandicapEntity handicap)
    {
        return new HandicapResponse
        {
            Id = handicap.Id,
            DiceRoll = handicap.DiceRoll,
            Name = handicap.Name,
            Data = MapperHelpers.FromJsonNotNull<HandicapData>(handicap.Data),
        };
    }
}
