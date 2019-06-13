using System.Collections.Generic;
using Naheulbook.Data.Models;
using Naheulbook.TestUtils;
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

        [Given("an item template category")]
        public void GivenAnItemTemplateCategory()
        {
            if (!_testDataUtil.Contains<ItemTemplateSection>())
                _testDataUtil.AddItemTemplateSection();
            _testDataUtil.AddItemTemplateCategory();
        }

        [Given("an item template")]
        public void GivenAnItemTemplate()
        {
            if (!_testDataUtil.Contains<ItemTemplateSection>())
                _testDataUtil.AddItemTemplateSection();
            if (!_testDataUtil.Contains<ItemTemplateCategory>())
                _testDataUtil.AddItemTemplateCategory();
            _testDataUtil.AddItemTemplate();
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
                .AddItemTemplateCategory();

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
                        RequireJob = _testDataUtil.GetLast<Job>(),
                        RequireOrigin = _testDataUtil.GetLast<Origin>(),
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