using System.Collections.Generic;
using Naheulbook.Data.Models;
using Naheulbook.Shared.TransientModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

// ReSharper disable MemberCanBeMadeStatic.Global

namespace Naheulbook.TestUtils;

public partial class DefaultEntityCreator
{
    public ItemTemplateEntity CreateItemTemplate(ItemTemplateSubCategoryEntity subCategory, string suffix = null)
    {
        if (suffix == null)
            suffix = RngUtil.GetRandomHexString(8);

        return new ItemTemplateEntity
        {
            SubCategory = subCategory,
            Data = @"{""key"": ""value""}",
            CleanName = $"some-item-name-{suffix}",
            Name = $"some-item-name-{suffix}",
            Source = "official",
            TechName = $"some-tech-name-{suffix}"
        };
    }

    public ItemTemplateSectionEntity CreateItemTemplateSection(string suffix = null)
    {
        if (suffix == null)
            suffix = RngUtil.GetRandomHexString(8);

        return new ItemTemplateSectionEntity
        {
            Name = $"some-item-name-{suffix}",
            Note = $"some-note-{suffix}",
            Special = $"some-special-{suffix}",
            Icon = $"some-icon-{suffix}"
        };
    }

    public ItemTemplateSubCategoryEntity CreateItemTemplateSubCategory(ItemTemplateSectionEntity section, string suffix = null)
    {
        if (suffix == null)
            suffix = RngUtil.GetRandomHexString(8);

        return new ItemTemplateSubCategoryEntity
        {
            Section = section,
            Note = $"some-note-{suffix}",
            Description = $"some-description-{suffix}",
            TechName = $"some-tech-name-{suffix}",
            Name = $"some-name-{suffix}"
        };
    }

    public SlotEntity CreateSlot(string suffix = null)
    {
        if (suffix == null)
            suffix = RngUtil.GetRandomHexString(8);

        return new SlotEntity
        {
            Count = 1,
            Name = $"some-name-{suffix}",
            TechName = $"some-tech-name-{suffix}"
        };
    }

    public ItemEntity CreateItem(ItemTemplateEntity itemTemplate)
    {
        return new ItemEntity
        {
            Data = """{"key":"value"}""",
            Modifiers = JsonConvert.SerializeObject(new List<ActiveStatsModifier>
                {
                    new ActiveStatsModifier
                    {
                        Active = true,
                        Description = "some-description",
                    },
                },
                new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                }),
            ItemTemplateId = itemTemplate.Id,
        };
    }

    public ItemEntity CreateItem(ItemTemplateEntity itemTemplate, CharacterEntity character)
    {
        return new ItemEntity
        {
            Data = @"{""key"":""value""}",
            Modifiers = JsonConvert.SerializeObject(new List<ActiveStatsModifier>
                {
                    new ActiveStatsModifier
                    {
                        Active = true,
                        Description = "some-description"
                    }
                },
                new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                }),
            ItemTemplateId = itemTemplate.Id,
            CharacterId = character.Id,
            Character = character
        };
    }

    public ItemEntity CreateItem(ItemTemplateEntity itemTemplate, MonsterEntity monster)
    {
        return new ItemEntity
        {
            Data = @"{""key"":""value""}",
            Modifiers = @"[{""key"":""value""}]",
            ItemTemplateId = itemTemplate.Id,
            MonsterId = monster.Id,
            Monster = monster
        };
    }

    public ItemEntity CreateItem(ItemTemplateEntity itemTemplate, LootEntity loot)
    {
        return new ItemEntity
        {
            Data = @"{""key"":""value""}",
            Modifiers = @"[{""key"":""value""}]",
            ItemTemplateId = itemTemplate.Id,
            LootId = loot.Id,
            Loot = loot
        };
    }

    public ItemTypeEntity CreateItemType(string suffix = null)
    {
        if (suffix == null)
            suffix = RngUtil.GetRandomHexString(8);

        return new ItemTypeEntity
        {
            DisplayName = $"some-display-name-{suffix}",
            TechName = $"some-tech-name-{suffix}"
        };
    }
}