using System.Collections.Generic;
using Naheulbook.Data.Models;
using Naheulbook.Shared.TransientModels;
using Naheulbook.TestUtils;
using Newtonsoft.Json;
using TechTalk.SpecFlow;

namespace Naheulbook.Tests.Functional.Code.SpecificSteps;

[Binding]
public class ItemTemplateSteps(TestDataUtil testDataUtil)
{
    [Given("a item type")]
    public void GivenAItemType()
    {
        testDataUtil.AddItemType();
    }

    [Given("an item slot")]
    public void GivenAnItemSlot()
    {
        testDataUtil.AddSlot();
    }

    [Given("an item template section")]
    public void GivenAnItemTemplateSection()
    {
        testDataUtil.AddItemTemplateSection();
    }

    [Given("an item template sub-category")]
    public void GivenAnItemTemplateSubCategory()
    {
        if (!testDataUtil.Contains<ItemTemplateSectionEntity>())
            testDataUtil.AddItemTemplateSection();
        testDataUtil.AddItemTemplateSubCategory();
    }

    [Given("item templates required for initial inventory")]
    public void GivenItemTemplatesRequiredForInitialInventory()
    {
        if (!testDataUtil.Contains<ItemTemplateSectionEntity>())
            testDataUtil.AddItemTemplateSection();
        if (!testDataUtil.Contains<ItemTemplateSubCategoryEntity>())
            testDataUtil.AddItemTemplateSubCategory();
        testDataUtil.AddItemTemplate(i => i.TechName = "MONEY");
        testDataUtil.AddItemTemplate(i => i.TechName = "BIG_PURSE");
        testDataUtil.AddItemTemplate(i => i.TechName = "MEDIUM_PURSE");
        testDataUtil.AddItemTemplate(i => i.TechName = "SMALL_PURSE");
    }

    [Given("an item template")]
    public void GivenAnItemTemplate()
    {
        if (!testDataUtil.Contains<ItemTemplateSectionEntity>())
            testDataUtil.AddItemTemplateSection();
        if (!testDataUtil.Contains<ItemTemplateSubCategoryEntity>())
            testDataUtil.AddItemTemplateSubCategory();
        testDataUtil.AddItemTemplate();
    }

    [Given("an item template with a charge")]
    public void GivenAnItemTemplateWithACharge()
    {
        if (!testDataUtil.Contains<ItemTemplateSectionEntity>())
            testDataUtil.AddItemTemplateSection();
        if (!testDataUtil.Contains<ItemTemplateSubCategoryEntity>())
            testDataUtil.AddItemTemplateSubCategory();
        testDataUtil.AddItemTemplate(itemTemplate =>
        {
            var itemTemplateData = JsonConvert.DeserializeObject<ItemTemplateData>(itemTemplate.Data?? "null") ?? new ItemTemplateData();
            itemTemplateData.Charge = 1;
            itemTemplateData.Actions = new List<NhbkAction>
            {
                new NhbkAction{Type = "addItem",Data = new NhbkActionData
                {
                    TemplateId = testDataUtil.GetLast<ItemTemplateEntity>().Id,
                }},
                new NhbkAction{Type = "addEv",Data = new NhbkActionData
                {
                    Ev = 1,
                }},
                new NhbkAction{Type = "addEa",Data = new NhbkActionData
                {
                    Ea = 1,
                }},
                new NhbkAction{Type = "custom",Data = new NhbkActionData
                {
                    Text = "some-text",
                }},
            };
            itemTemplate.Data = JsonConvert.SerializeObject(itemTemplateData);
        });
    }

    [Given("an item template with all optional fields set")]
    public void GivenAnItemTemplateWithAllOptionalFieldsSet()
    {
        testDataUtil
            .AddStat()
            .AddOrigin()
            .AddJob()
            .AddSlot()
            .AddSkill()
            .AddSkill()
            .AddSkill()
            .AddItemTemplateSection()
            .AddItemTemplateSubCategory();

        testDataUtil.AddItemTemplate(itemTemplate =>
        {
            itemTemplate.Requirements = new List<ItemTemplateRequirementEntity>
            {
                new ItemTemplateRequirementEntity
                {
                    Stat = testDataUtil.GetLast<StatEntity>(),
                    MinValue = 2,
                    MaxValue = 12,
                },
            };
            itemTemplate.Modifiers = new List<ItemTemplateModifierEntity>
            {
                new ItemTemplateModifierEntity
                {
                    Special = null,
                    RequiredJob = testDataUtil.GetLast<JobEntity>(),
                    RequiredOrigin = testDataUtil.GetLast<OriginEntity>(),
                    Stat = testDataUtil.GetLast<StatEntity>(),
                    Value = -2,
                    Type = "ADD",
                },
            };
            itemTemplate.Skills = new List<ItemTemplateSkillEntity>
            {
                new ItemTemplateSkillEntity
                {
                    Skill = testDataUtil.GetFromEnd<SkillEntity>(0),
                },
            };
            itemTemplate.UnSkills = new List<ItemTemplateUnSkillEntity>
            {
                new ItemTemplateUnSkillEntity
                {
                    Skill = testDataUtil.GetFromEnd<SkillEntity>(1),
                },
            };
            itemTemplate.Slots = new List<ItemTemplateSlotEntity>
            {
                new ItemTemplateSlotEntity
                {
                    Slot = testDataUtil.GetLast<SlotEntity>(),
                },
            };
            itemTemplate.SkillModifiers = new List<ItemTemplateSkillModifierEntity>
            {
                new ItemTemplateSkillModifierEntity
                {
                    Skill = testDataUtil.GetFromEnd<SkillEntity>(2),
                    Value = 2,
                },
            };
        });
    }
}