using System;
using System.Collections.Generic;
using Naheulbook.Data.Models;

namespace Naheulbook.TestUtils
{
    public partial class TestDataUtil
    {
        public TestDataUtil AddSlot(Action<Slot> customizer = null)
        {
            return SaveEntity(_defaultEntityCreator.CreateSlot(), customizer);
        }

        public TestDataUtil AddItemTemplateSection(Action<ItemTemplateSection> customizer = null)
        {
            return SaveEntity(_defaultEntityCreator.CreateItemTemplateSection(), customizer);
        }

        public TestDataUtil AddItemTemplateCategory(Action<ItemTemplateCategory> customizer = null)
        {
            return SaveEntity(_defaultEntityCreator.CreateItemTemplateCategory(GetLast<ItemTemplateSection>()), customizer);
        }

        public TestDataUtil AddItemTemplate(Action<ItemTemplate> customizer = null)
        {
            return SaveEntity(_defaultEntityCreator.CreateItemTemplate(GetLast<ItemTemplateCategory>()), customizer);
        }

        public TestDataUtil AddItemTemplateAndRequiredData(Action<ItemTemplate> customizer = null)
        {
            AddItemTemplateSection();
            AddItemTemplateCategory();
            return SaveEntity(_defaultEntityCreator.CreateItemTemplate(GetLast<ItemTemplateCategory>()), customizer);
        }

        public TestDataUtil AddItemTemplateWithAllData()
        {
            AddItemTemplateSection()
                .AddItemTemplateCategory()
                .AddStat()
                .AddJob()
                .AddOrigin()
                .AddSkill()
                .AddSkill()
                .AddSkill()
                .AddSlot();

            return AddItemTemplate(itemTemplate =>
            {
                itemTemplate.Requirements = new List<ItemTemplateRequirement>
                {
                    new ItemTemplateRequirement
                    {
                        Stat = Get<Stat>(),
                        MinValue = 2,
                        MaxValue = 12,
                    }
                };
                itemTemplate.Modifiers = new List<ItemTemplateModifier>
                {
                    new ItemTemplateModifier
                    {
                        Special = null,
                        RequireJob = Get<Job>(),
                        RequireOrigin = Get<Origin>(),
                        Stat = Get<Stat>(),
                        Value = 2,
                        Type = "ADD"
                    }
                };
                itemTemplate.Skills = new List<ItemTemplateSkill>
                {
                    new ItemTemplateSkill
                    {
                        Skill = Get<Skill>(0)
                    }
                };
                itemTemplate.UnSkills = new List<ItemTemplateUnSkill>
                {
                    new ItemTemplateUnSkill
                    {
                        Skill = Get<Skill>(1)
                    }
                };
                itemTemplate.Slots = new List<ItemTemplateSlot>
                {
                    new ItemTemplateSlot
                    {
                        Slot = Get<Slot>(0)
                    }
                };
                itemTemplate.SkillModifiers = new List<ItemTemplateSkillModifier>
                {
                    new ItemTemplateSkillModifier
                    {
                        Skill = Get<Skill>(2),
                        Value = 2
                    }
                };
            });
        }

        public TestDataUtil AddItem(Character character, Action<Item> customizer = null)
        {
            var item = _defaultEntityCreator.CreateItem(GetLast<ItemTemplate>(), character);

            return SaveEntity(item, customizer);
        }

        public TestDataUtil AddItem(Loot loot, Action<Item> customizer = null)
        {
            var item = _defaultEntityCreator.CreateItem(GetLast<ItemTemplate>(), loot);

            return SaveEntity(item, customizer);
        }

        public TestDataUtil AddItem(Monster monster, Action<Item> customizer = null)
        {
            var item = _defaultEntityCreator.CreateItem(GetLast<ItemTemplate>(), monster);

            return SaveEntity(item, customizer);
        }

        public TestDataUtil AddItemType(Action<ItemType> customizer = null)
        {
            return SaveEntity(_defaultEntityCreator.CreateItemType(), customizer);
        }
    }
}