using System;
using System.Collections.Generic;
using Naheulbook.Data.Models;

namespace Naheulbook.TestUtils
{
    public partial class TestDataUtil
    {
        public TestDataUtil AddSlot(Action<SlotEntity> customizer = null)
        {
            return SaveEntity(_defaultEntityCreator.CreateSlot(), customizer);
        }

        public TestDataUtil AddItemTemplateSection(Action<ItemTemplateSectionEntity> customizer = null)
        {
            return SaveEntity(_defaultEntityCreator.CreateItemTemplateSection(), customizer);
        }

        public TestDataUtil AddItemTemplateSubCategory(Action<ItemTemplateSubCategoryEntity> customizer = null)
        {
            return SaveEntity(_defaultEntityCreator.CreateItemTemplateSubCategory(GetLast<ItemTemplateSectionEntity>()), customizer);
        }

        public TestDataUtil AddItemTemplate(Action<ItemTemplateEntity> customizer = null)
        {
            return SaveEntity(_defaultEntityCreator.CreateItemTemplate(GetLast<ItemTemplateSubCategoryEntity>()), customizer);
        }

        public TestDataUtil AddItemTemplateAndRequiredData(Action<ItemTemplateEntity> customizer = null)
        {
            AddItemTemplateSection();
            AddItemTemplateSubCategory();
            return SaveEntity(_defaultEntityCreator.CreateItemTemplate(GetLast<ItemTemplateSubCategoryEntity>()), customizer);
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
                    new ItemTemplateRequirementEntity
                    {
                        Stat = Get<StatEntity>(),
                        MinValue = 2,
                        MaxValue = 12,
                    }
                };
                itemTemplate.Modifiers = new List<ItemTemplateModifierEntity>
                {
                    new ItemTemplateModifierEntity
                    {
                        Special = null,
                        RequiredJob = Get<JobEntity>(),
                        RequiredOrigin = Get<OriginEntity>(),
                        Stat = Get<StatEntity>(),
                        Value = 2,
                        Type = "ADD"
                    }
                };
                itemTemplate.Skills = new List<ItemTemplateSkillEntity>
                {
                    new ItemTemplateSkillEntity
                    {
                        Skill = Get<SkillEntity>(0)
                    }
                };
                itemTemplate.UnSkills = new List<ItemTemplateUnSkillEntity>
                {
                    new ItemTemplateUnSkillEntity
                    {
                        Skill = Get<SkillEntity>(1)
                    }
                };
                itemTemplate.Slots = new List<ItemTemplateSlotEntity>
                {
                    new ItemTemplateSlotEntity
                    {
                        Slot = Get<SlotEntity>(0)
                    }
                };
                itemTemplate.SkillModifiers = new List<ItemTemplateSkillModifierEntity>
                {
                    new ItemTemplateSkillModifierEntity
                    {
                        Skill = Get<SkillEntity>(2),
                        Value = 2
                    }
                };
            });
        }

        public TestDataUtil AddItem(CharacterEntity character, Action<ItemEntity> customizer = null)
        {
            var item = _defaultEntityCreator.CreateItem(GetLast<ItemTemplateEntity>(), character);

            return SaveEntity(item, customizer);
        }

        public TestDataUtil AddItem(LootEntity loot, Action<ItemEntity> customizer = null)
        {
            var item = _defaultEntityCreator.CreateItem(GetLast<ItemTemplateEntity>(), loot);

            return SaveEntity(item, customizer);
        }

        public TestDataUtil AddItem(MonsterEntity monster, Action<ItemEntity> customizer = null)
        {
            var item = _defaultEntityCreator.CreateItem(GetLast<ItemTemplateEntity>(), monster);

            return SaveEntity(item, customizer);
        }

        public TestDataUtil AddItemType(Action<ItemTypeEntity> customizer = null)
        {
            return SaveEntity(_defaultEntityCreator.CreateItemType(), customizer);
        }
    }
}