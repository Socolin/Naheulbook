using Naheulbook.Core.Features.Shared;
using Naheulbook.Data.EntityFrameworkCore.Entities;
using Naheulbook.Responses.Responses;
using Naheulbook.Shared.TransientModels;

namespace Naheulbook.Core.Features.SpecialAttack;

public interface ISpecialAttackMapper
{
    SpecialAttackResponse ToResponse(SpecialAttackEntity specialAttack);
    SpecialAttackGroupResponse ToResponse(SpecialAttackGroupEntity specialAttackGroup);
}

public class SpecialAttackMapper : ISpecialAttackMapper
{
    public SpecialAttackResponse ToResponse(SpecialAttackEntity specialAttack)
    {
        return new SpecialAttackResponse
        {
            Id = specialAttack.Id,
            Name = specialAttack.Name,
            Data = MapperHelpers.FromJsonNotNull<SpecialAttackData>(specialAttack.Data),
        };
    }

    public SpecialAttackGroupResponse ToResponse(SpecialAttackGroupEntity specialAttackGroup)
    {
        return new SpecialAttackGroupResponse
        {
            Id = specialAttackGroup.Id,
            Name = specialAttackGroup.Name,
            Description = specialAttackGroup.Description,
            SpecialAttacks = specialAttackGroup.SpecialAttacks
                .OrderBy(x => x.DisplayOrder)
                .Select(ToResponse)
                .ToList(),
        };
    }
}
