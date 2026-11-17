using Naheulbook.Core.Features.Monster;
using Naheulbook.Data.EntityFrameworkCore.Entities;
using Naheulbook.Responses.Responses;

namespace Naheulbook.Core.Features.Fight;

public interface IFightMapper
{
    FightResponse ToResponse(FightEntity fight);
}

public class FightMapper(IMonsterMapper monsterMapper) : IFightMapper
{
    public FightResponse ToResponse(FightEntity fight)
    {
        return new FightResponse
        {
            Id = fight.Id,
            Name = fight.Name,
            Monsters = fight.Monsters.Select(monsterMapper.ToResponse).ToList(),
        };
    }
}
