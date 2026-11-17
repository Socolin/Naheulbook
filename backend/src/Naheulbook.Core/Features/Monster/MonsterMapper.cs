using Naheulbook.Core.Features.Item;
using Naheulbook.Core.Features.Shared;
using Naheulbook.Data.EntityFrameworkCore.Entities;
using Naheulbook.Responses.Responses;
using Naheulbook.Shared.TransientModels;
using Newtonsoft.Json.Linq;

namespace Naheulbook.Core.Features.Monster;

public interface IMonsterMapper
{
    MonsterResponse ToResponse(MonsterEntity monster);
    DeadMonsterResponse ToDeadResponse(MonsterEntity monster);
}

public class MonsterMapper(IItemMapper itemMapper) : IMonsterMapper
{
    public MonsterResponse ToResponse(MonsterEntity monster)
    {
        return new MonsterResponse
        {
            Id = monster.Id,
            FightId = monster.FightId,
            Name = monster.Name,
            Dead = MapperHelpers.FromDateTimeToString(monster.Dead),
            Data = MapperHelpers.FromJson<JObject>(monster.Data) ?? new JObject(),
            Modifiers = MapperHelpers.FromJson<List<ActiveStatsModifier>>(monster.Modifiers) ?? [],
            Items = monster.Items.Select(itemMapper.ToResponse).ToList(),
        };
    }

    public DeadMonsterResponse ToDeadResponse(MonsterEntity monster)
    {
        return new DeadMonsterResponse
        {
            Id = monster.Id,
            Name = monster.Name,
            Dead = MapperHelpers.FromDateTimeToString(monster.Dead),
            Data = MapperHelpers.FromJson<JObject>(monster.Data) ?? new JObject(),
        };
    }
}
