using System.Collections.Generic;
using Naheulbook.Data.Models;
using Naheulbook.Shared.TransientModels;
using Naheulbook.TestUtils;
using Newtonsoft.Json;
using TechTalk.SpecFlow;

namespace Naheulbook.Tests.Functional.Code.SpecificSteps
{
    [Binding]
    public class ItemTemplateSteps
    {
        private readonly TestDataUtil _testDataUtil;

        public ItemTemplateSteps(
            TestDataUtil testDataUtil
        )
        {
            _testDataUtil = testDataUtil;
        }

        [Given("a item type")]
        public void GivenAItemType()
        {
            _testDataUtil.AddItemType();
        }

        [Given("an item slot")]
        public void GivenAnItemSlot()
        {
            _testDataUtil.AddSlot();
        }

        [Given("an item template section")]
        public void GivenAnItemTemplateSection()
        {
            _testDataUtil.AddItemTemplateSection();
        }

        [Given("an item template sub-category")]
        public void GivenAnItemTemplateSubCategory()
        {
            if (!_testDataUtil.Contains<ItemTemplateSection>())
                _testDataUtil.AddItemTemplateSection();
            _testDataUtil.AddItemTemplateSubCategory();
        }

        [Given("item templates required for initial inventory")]
        public void GivenItemTemplatesRequiredForInitialInventory()
        {
            if (!_testDataUtil.Contains<ItemTemplateSection>())
                _testDataUtil.AddItemTemplateSection();
            if (!_testDataUtil.Contains<ItemTemplateSubCategory>())
                _testDataUtil.AddItemTemplateSubCategory();
            _testDataUtil.AddItemTemplate(i => i.TechName = "MONEY");
            _testDataUtil.AddItemTemplate(i => i.TechName = "BIG_PURSE");
            _testDataUtil.AddItemTemplate(i => i.TechName = "MEDIUM_PURSE");
            _testDataUtil.AddItemTemplate(i => i.TechName = "SMALL_PURSE");
        }

        [Given("an item template")]
        public void GivenAnItemTemplate()
        {
            if (!_testDataUtil.Contains<ItemTemplateSection>())
                _testDataUtil.AddItemTemplateSection();
            if (!_testDataUtil.Contains<ItemTemplateSubCategory>())
                _testDataUtil.AddItemTemplateSubCategory();
            _testDataUtil.AddItemTemplate();
        }

        [Given("an item template with a charge")]
        public void GivenAnItemTemplateWithACharge()
        {
            if (!_testDataUtil.Contains<ItemTemplateSection>())
                _testDataUtil.AddItemTemplateSection();
            if (!_testDataUtil.Contains<ItemTemplateSubCategory>())
                _testDataUtil.AddItemTemplateSubCategory();
            _testDataUtil.AddItemTemplate(itemTemplate =>
            {
                var itemTemplateData = JsonConvert.DeserializeObject<ItemTemplateData>(itemTemplate.Data) ?? new ItemTemplateData();
                itemTemplateData.Charge = 1;
                itemTemplateData.Actions = new List<NhbkAction>
                {
                    new NhbkAction{Type = "addItem",Data = new NhbkActionData
                    {
                        TemplateId = _testDataUtil.GetLast<ItemTemplate>().Id,
                    }},
                    new NhbkAction{Type = "addEv",Data = new NhbkActionData
                    {
                        Ev = 1
                    }},
                    new NhbkAction{Type = "addEa",Data = new NhbkActionData
                    {
                        Ea = 1
                    }},
                    new NhbkAction{Type = "custom",Data = new NhbkActionData
                    {
                        Text = "some-text"
                    }}
                };
                itemTemplate.Data = JsonConvert.SerializeObject(itemTemplateData);
            });
        }

        [Given("an item template with all optional fields set")]
        public void GivenAnItemTemplateWithAllOptionalFieldsSet()
        {
            _testDataUtil
                .AddStat()
                .AddOrigin()
                .AddJob()
                .AddSlot()
                .AddSkill()
                .AddSkill()
                .AddSkill()
                .AddItemTemplateSection()
                .AddItemTemplateSubCategory();

            _testDataUtil.AddItemTemplate(itemTemplate =>
            {
                itemTemplate.Requirements = new List<ItemTemplateRequirement>
                {
                    new ItemTemplateRequirement
                    {
                        Stat = _testDataUtil.GetLast<Stat>(),
                        MinValue = 2,
                        MaxValue = 12,
                    }
                };
                itemTemplate.Modifiers = new List<ItemTemplateModifier>
                {
                    new ItemTemplateModifier
                    {
                        Special = null,
                        RequiredJob = _testDataUtil.GetLast<Job>(),
                        RequiredOrigin = _testDataUtil.GetLast<Origin>(),
                        Stat = _testDataUtil.GetLast<Stat>(),
                        Value = -2,
                        Type = "ADD",
                    }
                };
                itemTemplate.Skills = new List<ItemTemplateSkill>
                {
                    new ItemTemplateSkill
                    {
                        Skill = _testDataUtil.GetFromEnd<Skill>(0)
                    }
                };
                itemTemplate.UnSkills = new List<ItemTemplateUnSkill>
                {
                    new ItemTemplateUnSkill
                    {
                        Skill = _testDataUtil.GetFromEnd<Skill>(1)
                    }
                };
                itemTemplate.Slots = new List<ItemTemplateSlot>
                {
                    new ItemTemplateSlot
                    {
                        Slot = _testDataUtil.GetLast<Slot>()
                    }
                };
                itemTemplate.SkillModifiers = new List<ItemTemplateSkillModifier>
                {
                    new ItemTemplateSkillModifier
                    {
                        Skill = _testDataUtil.GetFromEnd<Skill>(2),
                        Value = 2
                    }
                };
            });
        }
    }
}