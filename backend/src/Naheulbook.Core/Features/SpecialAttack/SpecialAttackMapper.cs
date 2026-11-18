using Naheulbook.Core.Features.Shared;
using Naheulbook.Data.EntityFrameworkCore.Entities;
using Naheulbook.Responses.Responses;
using Naheulbook.Shared.TransientModels;

namespace Naheulbook.Core.Features.SpecialAttack;

public interface ISpecialAttackMapper
{
    SpecialAttackResponse ToResponse(SpecialAttackEntity specialAttack);
    SpecialAttackGroupResponse ToResponse(SpecialAttackGroupEntity specialAttackGroup);
    CharacterSpecialAttackResponse ToResponse(CharacterSpecialAttackEntity characterSpecialAttack);
    UseCharacterSpecialAttackResponse ToResponse(UseSpecialAttackResult useSpecialAttackResult);
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

    public CharacterSpecialAttackResponse ToResponse(CharacterSpecialAttackEntity characterSpecialAttack)
    {
        return new CharacterSpecialAttackResponse
        {
            SpecialAttack = ToResponse(characterSpecialAttack.SpecialAttack),
            Data = MapperHelpers.FromJsonNotNull<CharacterSpecialAttackData>(characterSpecialAttack.Data),
        };
    }

    public UseCharacterSpecialAttackResponse ToResponse(UseSpecialAttackResult useSpecialAttackResult)
    {
        return new UseCharacterSpecialAttackResponse
        {
            SpecialAttack = ToResponse(useSpecialAttackResult.CharacterSpecialAttack.SpecialAttack),
            Data = MapperHelpers.FromJsonNotNull<CharacterSpecialAttackData>(useSpecialAttackResult.CharacterSpecialAttack.Data),
            Experience = useSpecialAttackResult.Experience,
        };
    }
}
