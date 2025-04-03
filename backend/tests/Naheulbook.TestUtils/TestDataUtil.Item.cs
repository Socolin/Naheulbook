using System;
using System.Collections.Generic;
using Naheulbook.Data.Models;

namespace Naheulbook.TestUtils;

public partial class TestDataUtil
{
    public TestDataUtil AddSlot(Action<SlotEntity> customizer = null)
    {
        return AddSlot(out _, customizer);
    }

    public TestDataUtil AddSlot(out SlotEntity slot, Action<SlotEntity> customizer = null)
    {
        slot = defaultEntityCreator.CreateSlot();
        return SaveEntity(slot, customizer);
    }

    public TestDataUtil AddItemTemplateSection(Action<ItemTemplateSectionEntity> customizer = null)
    {
        return SaveEntity(defaultEntityCreator.CreateItemTemplateSection(), customizer);
    }

    public TestDataUtil AddItemTemplateSubCategory(Action<ItemTemplateSubCategoryEntity> customizer = null)
    {
        return SaveEntity(defaultEntityCreator.CreateItemTemplateSubCategory(GetLast<ItemTemplateSectionEntity>()), customizer);
    }

    public TestDataUtil AddItemTemplate(Action<ItemTemplateEntity> customizer = null)
    {
        return AddItemTemplate(out _, customizer);
    }

    public TestDataUtil AddItemTemplate(out ItemTemplateEntity itemTemplate, Action<ItemTemplateEntity> customizer = null)
    {
        itemTemplate = defaultEntityCreator.CreateItemTemplate(GetLast<ItemTemplateSubCategoryEntity>());

        return SaveEntity(itemTemplate, customizer);
    }

    public TestDataUtil AddItemTemplateAndRequiredData(Action<ItemTemplateEntity> customizer = null)
    {
        AddItemTemplateSection();
        AddItemTemplateSubCategory();
        return SaveEntity(defaultEntityCreator.CreateItemTemplate(GetLast<ItemTemplateSubCategoryEntity>()), customizer);
    }

    public TestDataUtil AddItemTemplateWithAllData()
    {
        AddItemTemplateSection()
            .AddItemTemplateSubCategory()
            .AddStat()
            .AddJob()
            .AddOrigin()
            .AddSkill()
            .AddSkill()
            .AddSkill()
            .AddSlot();

        return AddItemTemplate(itemTemplate =>
        {
            itemTemplate.Requirements = new List<ItemTemplateRequirementEntity>
            {
                new()
                {
                    Stat = Get<StatEntity>(),
                    MinValue = 2,
                    MaxValue = 12,
                },
            };
            itemTemplate.Modifiers = new List<ItemTemplateModifierEntity>
            {
                new()
                {
                    Special = null,
                    RequiredJob = Get<JobEntity>(),
                    RequiredOrigin = Get<OriginEntity>(),
                    Stat = Get<StatEntity>(),
                    Value = 2,
                    Type = "ADD",
                },
            };
            itemTemplate.Skills = new List<ItemTemplateSkillEntity>
            {
                new()
                {
                    Skill = Get<SkillEntity>(0),
                },
            };
            itemTemplate.UnSkills = new List<ItemTemplateUnSkillEntity>
            {
                new()
                {
                    Skill = Get<SkillEntity>(1),
                },
            };
            itemTemplate.Slots = new List<ItemTemplateSlotEntity>
            {
                new()
                {
                    Slot = Get<SlotEntity>(0),
                },
            };
            itemTemplate.SkillModifiers = new List<ItemTemplateSkillModifierEntity>
            {
                new()
                {
                    Skill = Get<SkillEntity>(2),
                    Value = 2,
                },
            };
        });
    }

    public TestDataUtil AddItem(CharacterEntity character, Action<ItemEntity> customizer = null)
    {
        return AddItem(out _,
            i =>
            {
                i.CharacterId = character.Id;
                customizer?.Invoke(i);
            });
    }

    public TestDataUtil AddItemToLoot(LootEntity loot = null, Action<ItemEntity> customizer = null)
    {
        return AddItemToLoot(out _, loot, customizer);
    }

    public TestDataUtil AddItemToLoot(out ItemEntity item, LootEntity loot = null, Action<ItemEntity> customizer = null)
    {
        return AddItem(out item,
            i =>
            {
                i.LootId = loot?.Id ?? GetLast<LootEntity>().Id;
                customizer?.Invoke(i);
            });
    }

    public TestDataUtil AddItemToMonster(MonsterEntity monster = null, Action<ItemEntity> customizer = null)
    {
        return AddItemToMonster(out _, monster, customizer);
    }

    public TestDataUtil AddItemToMonster(out ItemEntity item, MonsterEntity monster = null, Action<ItemEntity> customizer = null)
    {
        return AddItem(out item,
            i =>
            {
                i.MonsterId = monster?.Id ?? GetLast<MonsterEntity>().Id;
                customizer?.Invoke(i);
            });
    }

    public TestDataUtil AddItemToCharacter(CharacterEntity character = null, Action<ItemEntity> customizer = null)
    {
        return AddItemToCharacter(out _, character, customizer);
    }

    public TestDataUtil AddItemToCharacter(out ItemEntity item, CharacterEntity character = null, Action<ItemEntity> customizer = null)
    {
        return AddItem(out item,
            i =>
            {
                i.CharacterId = character?.Id ?? GetLast<CharacterEntity>().Id;
                customizer?.Invoke(i);
            });
    }

    public TestDataUtil AddItem(out ItemEntity item, LootEntity loot, Action<ItemEntity> customizer = null)
    {
        return AddItem(out item,
            i =>
            {
                i.LootId = loot.Id;
                customizer?.Invoke(i);
            });
    }

    public TestDataUtil AddItem(out ItemEntity item, MonsterEntity monster, Action<ItemEntity> customizer = null)
    {
        return AddItem(out item,
            i =>
            {
                i.MonsterId = monster.Id;
                customizer?.Invoke(i);
            });
    }

    public TestDataUtil AddItem(out ItemEntity item, Action<ItemEntity> customizer = null)
    {
        item = defaultEntityCreator.CreateItem(GetLast<ItemTemplateEntity>());

        return SaveEntity(item, customizer);
    }

    public TestDataUtil AddItemType(Action<ItemTypeEntity> customizer = null)
    {
        return SaveEntity(defaultEntityCreator.CreateItemType(), customizer);
    }

    public TestDataUtil AddItemTemplateRequirement(Action<ItemTemplateRequirementEntity> customizer = null)
    {
        return AddItemTemplateRequirement(out _, customizer);
    }

    public TestDataUtil AddItemTemplateRequirement(
        out ItemTemplateRequirementEntity itemTemplateRequirement,
        Action<ItemTemplateRequirementEntity> customizer = null
    )
    {
        var itemTemplate = GetLast<ItemTemplateEntity>();
        var stat = GetLast<StatEntity>();
        itemTemplateRequirement = new ItemTemplateRequirementEntity
        {
            ItemTemplateId = itemTemplate.Id,
            StatName = stat.Name,
            MinValue = 3,
            MaxValue = 15,
        };
        return SaveEntity(itemTemplateRequirement, customizer);
    }

    public TestDataUtil AddItemTemplateModifier(Action<ItemTemplateModifierEntity> customizer = null)
    {
        return AddItemTemplateModifier(out _, customizer);
    }

    public TestDataUtil AddItemTemplateModifier(
        out ItemTemplateModifierEntity itemTemplateRequirement,
        Action<ItemTemplateModifierEntity> customizer = null
    )
    {
        var itemTemplate = GetLast<ItemTemplateEntity>();
        var stat = GetLast<StatEntity>();
        itemTemplateRequirement = new ItemTemplateModifierEntity
        {
            ItemTemplateId = itemTemplate.Id,
            StatName = stat.Name,
            Value = Random.Shared.Next(0, 32),
            Type = "some-type",
            Special = "some-special",
        };
        return SaveEntity(itemTemplateRequirement, customizer);
    }

    public TestDataUtil AddItemTemplateSkill(Action<ItemTemplateSkillEntity> customizer = null)
    {
        return AddItemTemplateSkill(out _, customizer);
    }

    public TestDataUtil AddItemTemplateSkill(
        out ItemTemplateSkillEntity itemTemplateSkill,
        Action<ItemTemplateSkillEntity> customizer = null
    )
    {
        var skill = GetLast<SkillEntity>();
        return AddItemTemplateSkill(out itemTemplateSkill, skill, customizer);
    }

    public TestDataUtil AddItemTemplateSkill(
        out ItemTemplateSkillEntity itemTemplateSkill,
        SkillEntity skill,
        Action<ItemTemplateSkillEntity> customizer = null
    )
    {
        var itemTemplate = GetLast<ItemTemplateEntity>();
        itemTemplateSkill = new ItemTemplateSkillEntity
        {
            ItemTemplateId = itemTemplate.Id,
            SkillId = skill.Id,
        };
        return SaveEntity(itemTemplateSkill, customizer);
    }

    public TestDataUtil AddItemTemplateUnSkill(Action<ItemTemplateUnSkillEntity> customizer = null)
    {
        return AddItemTemplateUnSkill(out _, customizer);
    }

    public TestDataUtil AddItemTemplateUnSkill(
        out ItemTemplateUnSkillEntity itemTemplateUnSkill,
        Action<ItemTemplateUnSkillEntity> customizer = null
    )
    {
        var skill = GetLast<SkillEntity>();
        return AddItemTemplateUnSkill(out itemTemplateUnSkill, skill, customizer);
    }

    public TestDataUtil AddItemTemplateUnSkill(
        out ItemTemplateUnSkillEntity itemTemplateUnSkill,
        SkillEntity skill,
        Action<ItemTemplateUnSkillEntity> customizer = null
    )
    {
        var itemTemplate = GetLast<ItemTemplateEntity>();
        itemTemplateUnSkill = new ItemTemplateUnSkillEntity
        {
            ItemTemplateId = itemTemplate.Id,
            SkillId = skill.Id,
        };
        return SaveEntity(itemTemplateUnSkill, customizer);
    }

    public TestDataUtil AddItemTemplateSkillModifier(Action<ItemTemplateSkillModifierEntity> customizer = null)
    {
        return AddItemTemplateSkillModifier(out _, customizer);
    }

    public TestDataUtil AddItemTemplateSkillModifier(
        out ItemTemplateSkillModifierEntity itemTemplateSkillModifier,
        Action<ItemTemplateSkillModifierEntity> customizer = null
    )
    {
        var skill = GetLast<SkillEntity>();
        return AddItemTemplateSkillModifier(out itemTemplateSkillModifier, skill, customizer);
    }

    public TestDataUtil AddItemTemplateSkillModifier(
        out ItemTemplateSkillModifierEntity itemTemplateSkillModifier,
        SkillEntity skill,
        Action<ItemTemplateSkillModifierEntity> customizer = null
    )
    {
        var itemTemplate = GetLast<ItemTemplateEntity>();
        itemTemplateSkillModifier = new ItemTemplateSkillModifierEntity
        {
            ItemTemplateId = itemTemplate.Id,
            SkillId = skill.Id,
        };
        return SaveEntity(itemTemplateSkillModifier, customizer);
    }


    public TestDataUtil AddItemTemplateSlot(Action<ItemTemplateSlotEntity> customizer = null)
    {
        return AddItemTemplateSlot(out _, customizer);
    }

    public TestDataUtil AddItemTemplateSlot(
        out ItemTemplateSlotEntity itemTemplateRequirement,
        Action<ItemTemplateSlotEntity> customizer = null
    )
    {
        var itemTemplate = GetLast<ItemTemplateEntity>();
        var slot = GetLast<SlotEntity>();
        itemTemplateRequirement = new ItemTemplateSlotEntity
        {
            ItemTemplateId = itemTemplate.Id,
            SlotId = slot.Id,
        };
        return SaveEntity(itemTemplateRequirement, customizer);
    }
}