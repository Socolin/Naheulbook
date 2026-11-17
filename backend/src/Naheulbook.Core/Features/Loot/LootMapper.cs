using Naheulbook.Core.Features.Item;
using Naheulbook.Core.Features.Monster;
using Naheulbook.Data.EntityFrameworkCore.Entities;
using Naheulbook.Responses.Responses;

namespace Naheulbook.Core.Features.Loot;

public interface ILootMapper
{
    LootResponse ToResponse(LootEntity loot);
}

public class LootMapper(IItemMapper itemMapper, IMonsterMapper monsterMapper) : ILootMapper
{
    public LootResponse ToResponse(LootEntity loot)
    {
        return new LootResponse
        {
            Id = loot.Id,
            Name = loot.Name,
            IsVisibleForPlayer = loot.IsVisibleForPlayer,
            Items = loot.Items.Select(itemMapper.ToResponse).ToList(),
            Monsters = loot.Monsters.Select(monsterMapper.ToResponse).ToList(),
        };
    }
}
