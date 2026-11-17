using Naheulbook.Core.Features.Shared;
using Naheulbook.Data.EntityFrameworkCore.Entities;
using Naheulbook.Responses.Responses;
using Naheulbook.Shared.TransientModels;
using Newtonsoft.Json.Linq;

namespace Naheulbook.Core.Features.Item;

public interface IItemMapper
{
    ItemResponse ToResponse(ItemEntity item);
    ItemPartialResponse ToPartialResponse(ItemEntity item);
}

public class ItemMapper(IItemTemplateMapper itemTemplateMapper) : IItemMapper
{
    public ItemResponse ToResponse(ItemEntity item)
    {
        return new ItemResponse
        {
            Id = item.Id,
            Data = MapperHelpers.FromJson<JObject>(item.Data) ?? new JObject(),
            Modifiers = MapperHelpers.FromJson<List<ActiveStatsModifier>>(item.Modifiers) ?? [],
            ContainerId = item.ContainerId,
            ItemTemplate = itemTemplateMapper.ToResponse(item.ItemTemplate),
        };
    }

    public ItemPartialResponse ToPartialResponse(ItemEntity item)
    {
        return new ItemPartialResponse
        {
            Id = item.Id,
            Data = MapperHelpers.FromJson<JObject>(item.Data) ?? new JObject(),
            Modifiers = MapperHelpers.FromJson<List<ActiveStatsModifier>>(item.Modifiers) ?? [],
            ContainerId = item.ContainerId,
        };
    }
}
