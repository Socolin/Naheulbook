using Naheulbook.Core.Features.Item;
using Naheulbook.Core.Features.Shared;
using Naheulbook.Data.EntityFrameworkCore.Entities;
using Naheulbook.Responses.Responses;
using Newtonsoft.Json.Linq;

namespace Naheulbook.Core.Features.Monster;

public interface IMonsterTemplateMapper
{
    MonsterTemplateResponse ToResponse(MonsterTemplateEntity monsterTemplate);
    MonsterTemplateResponse.MonsterTemplateInventoryElementResponse ToResponse(MonsterTemplateInventoryElementEntity inventoryElement);
    MonsterTypeResponse ToResponse(MonsterTypeEntity monsterType);
    MonsterSubCategoryResponse ToResponse(MonsterSubCategoryEntity monsterSubCategory);
    MonsterTraitResponse ToResponse(MonsterTraitEntity monsterTrait);
}

public class MonsterTemplateMapper(IItemTemplateMapper itemTemplateMapper) : IMonsterTemplateMapper
{
    public MonsterTemplateResponse ToResponse(MonsterTemplateEntity monsterTemplate)
    {
        return new MonsterTemplateResponse
        {
            Id = monsterTemplate.Id,
            Name = monsterTemplate.Name,
            SubCategoryId = monsterTemplate.SubCategoryId,
            Data = MapperHelpers.FromJson<JObject>(monsterTemplate.Data) ?? new JObject(),
            Inventory = monsterTemplate.Items.Select(ToResponse).ToList(),
        };
    }

    public MonsterTemplateResponse.MonsterTemplateInventoryElementResponse ToResponse(MonsterTemplateInventoryElementEntity inventoryElement)
    {
        return new MonsterTemplateResponse.MonsterTemplateInventoryElementResponse
        {
            Id = inventoryElement.Id,
            ItemTemplate = itemTemplateMapper.ToResponse(inventoryElement.ItemTemplate),
            MinCount = inventoryElement.MinCount,
            MaxCount = inventoryElement.MaxCount,
            Chance = inventoryElement.Chance,
        };
    }

    public MonsterTypeResponse ToResponse(MonsterTypeEntity monsterType)
    {
        return new MonsterTypeResponse
        {
            Id = monsterType.Id,
            Name = monsterType.Name,
            SubCategories = monsterType.SubCategories.OrderBy(c => c.Id).Select(ToResponse).ToList(),
        };
    }

    public MonsterSubCategoryResponse ToResponse(MonsterSubCategoryEntity monsterSubCategory)
    {
        return new MonsterSubCategoryResponse
        {
            Id = monsterSubCategory.Id,
            Name = monsterSubCategory.Name,
            TypeId = monsterSubCategory.TypeId,
        };
    }

    public MonsterTraitResponse ToResponse(MonsterTraitEntity monsterTrait)
    {
        return new MonsterTraitResponse
        {
            Id = monsterTrait.Id,
            Name = monsterTrait.Name,
            Description = monsterTrait.Description,
            Levels = MapperHelpers.FromJson<List<string>>(monsterTrait.Levels) ?? [],
        };
    }
}
