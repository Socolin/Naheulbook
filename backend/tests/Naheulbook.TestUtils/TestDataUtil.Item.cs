using System;
using System.Collections.Generic;
using Naheulbook.Data.EntityFrameworkCore.Entities;
using Naheulbook.Shared.TransientModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Naheulbook.TestUtils;

public partial class TestDataUtil
{
    public TestDataUtil AddSlot(Action<SlotEntity> customizer = null)
    {
        return AddSlot(out _, customizer);
    }

    public TestDataUtil AddSlot(out SlotEntity slot, Action<SlotEntity> customizer = null)
    {
        slot = new SlotEntity
        {
            Count = 1,
            Name = RngUtil.GetRandomString("some-name"),
            TechName = RngUtil.GetRandomString("some-tech-name"),
        };

        return SaveEntity(slot, customizer);
    }

    public TestDataUtil AddItemTemplateSection(Action<ItemTemplateSectionEntity> customizer = null)
    {
        return AddItemTemplateSection(out _, customizer);
    }

    public TestDataUtil AddItemTemplateSection(
        out ItemTemplateSectionEntity itemTemplateSection,
        Action<ItemTemplateSectionEntity> customizer = null
    )
    {
        itemTemplateSection = new ItemTemplateSectionEntity
        {
            Name = RngUtil.GetRandomString("some-item-name"),
            Note = RngUtil.GetRandomString("some-note"),
            Special = RngUtil.GetRandomString("some-special"),
            Icon = RngUtil.GetRandomString("some-icon"),
        };

        return SaveEntity(itemTemplateSection, customizer);
    }

    public TestDataUtil AddItemTemplateSubCategory(Action<ItemTemplateSubCategoryEntity> customizer = null)
    {
        return AddItemTemplateSubCategory(out _, customizer);
    }

    public TestDataUtil AddItemTemplateSubCategory(out ItemTemplateSubCategoryEntity itemTemplateSubCategory, Action<ItemTemplateSubCategoryEntity> customizer = null)
    {
        var itemTemplateSection = GetLast<ItemTemplateSectionEntity>();

        itemTemplateSubCategory = new ItemTemplateSubCategoryEntity
        {
            SectionId = itemTemplateSection.Id,
            Note = RngUtil.GetRandomString("some-note"),
            Description = RngUtil.GetRandomString("some-description"),
            TechName = RngUtil.GetRandomString("some-tech-name"),
            Name = RngUtil.GetRandomString("some-name"),
        };

        return SaveEntity(itemTemplateSubCategory, customizer);
    }

    public TestDataUtil AddItemTemplate(Action<ItemTemplateEntity> customizer = null)
    {
        return AddItemTemplate(out _, customizer);
    }

    public TestDataUtil AddItemTemplate(out ItemTemplateEntity itemTemplate, Action<ItemTemplateEntity> customizer = null)
    {
        var itemTemplateSubCategory = GetLast<ItemTemplateSubCategoryEntity>();

        var itemName = RngUtil.GetRandomString("some-item-name");
        itemTemplate = new ItemTemplateEntity
        {
            SubCategory = itemTemplateSubCategory,
            Data = """{"key": "value"}""",
            CleanName = itemName,
            Name = itemName,
            Source = "official",
            TechName = RngUtil.GetRandomString("some-tech-name"),
        };

        return SaveEntity(itemTemplate, customizer);
    }

    public TestDataUtil AddItemTemplateAndRequiredData(Action<ItemTemplateEntity> customizer = null)
    {
        return AddItemTemplateSection()
            .AddItemTemplateSubCategory()
            .AddItemTemplate();
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
            }
        );
    }

    public TestDataUtil AddItem(CharacterEntity character, Action<ItemEntity> customizer = null)
    {
        return AddItem(out _,
            i =>
            {
                i.CharacterId = character.Id;
                customizer?.Invoke(i);
            }
        );
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
            }
        );
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
            }
        );
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
            }
        );
    }

    public TestDataUtil AddItem(out ItemEntity item, LootEntity loot, Action<ItemEntity> customizer = null)
    {
        return AddItem(out item,
            i =>
            {
                i.LootId = loot.Id;
                customizer?.Invoke(i);
            }
        );
    }

    public TestDataUtil AddItem(out ItemEntity item, MonsterEntity monster, Action<ItemEntity> customizer = null)
    {
        return AddItem(out item,
            i =>
            {
                i.MonsterId = monster.Id;
                customizer?.Invoke(i);
            }
        );
    }

    public TestDataUtil AddItem(out ItemEntity item, Action<ItemEntity> customizer = null)
    {
        var itemTemplate = GetLast<ItemTemplateEntity>();

        item = new ItemEntity
        {
            Data = """{"key":"value"}""",
            Modifiers = JsonConvert.SerializeObject(new List<ActiveStatsModifier>
                {
                    new()
                    {
                        Active = true,
                        Description = "some-description",
                    },
                },
                new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                }
            ),
            ItemTemplateId = itemTemplate.Id,
        };

        return SaveEntity(item, customizer);
    }

    public TestDataUtil AddItemType(Action<ItemTypeEntity> customizer = null)
    {
        return AddItemType(out _, customizer);
    }

    public TestDataUtil AddItemType(out ItemTypeEntity itemType, Action<ItemTypeEntity> customizer = null)
    {
        itemType = new ItemTypeEntity
        {
            DisplayName = RngUtil.GetRandomString("some-display-name"),
            TechName = RngUtil.GetRandomString("some-tech-name"),
        };

        return SaveEntity(itemType, customizer);
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