using System;
using System.Collections.Generic;
using Naheulbook.Data.EntityFrameworkCore.Entities;
using Naheulbook.Shared.TransientModels;
using Naheulbook.TestUtils;
using Newtonsoft.Json;
using Reqnroll;

namespace Naheulbook.Tests.Functional.Code.SpecificSteps;

[Binding]
public class MonsterSteps(TestDataUtil testDataUtil)
{
    [Given("^a monster category type$")]
    public void GivenAMonsterCategoryType()
    {
        testDataUtil.AddMonsterType();
    }

    [Given("^a monster sub-category$")]
    public void GivenAMonsterSubCategory()
    {
        testDataUtil.AddMonsterSubCategory();
    }

    [Given("^a monster trait$")]
    public void GivenAMonsterTrait()
    {
        testDataUtil.AddMonsterTrait();
    }

    [Given("^a monster template$")]
    public void GivenAMonsterTemplate()
    {
        if (!testDataUtil.Contains<MonsterTypeEntity>())
            testDataUtil.AddMonsterType();
        if (!testDataUtil.Contains<MonsterSubCategoryEntity>())
            testDataUtil.AddMonsterSubCategory();
        testDataUtil.AddMonsterTemplate();
    }

    [Given("^a monster template with inventory$")]
    public void GivenAMonsterTemplateWithLocationAndInventory()
    {
        if (!testDataUtil.Contains<MonsterTypeEntity>())
            testDataUtil.AddMonsterType();
        if (!testDataUtil.Contains<MonsterSubCategoryEntity>())
            testDataUtil.AddMonsterSubCategory();

        testDataUtil.AddItemTemplateSection().AddItemTemplateSubCategory().AddItemTemplate();
        testDataUtil.AddMonsterTemplate(m =>
        {
            m.Items = new List<MonsterTemplateInventoryElementEntity>
            {
                new()
                {
                    Chance = 0.5f,
                    ItemTemplate = testDataUtil.GetLast<ItemTemplateEntity>(),
                    MinCount = 1,
                    MaxCount = 3,
                },
            };
        });
    }

    [Given("^a monster$")]
    public void GivenAMonster()
    {
        testDataUtil.AddMonster();
    }

    [Given(@"^a monster with a modifier active for (\d) laps?$")]
    public void GivenAMonsterWithAModifierActiveForXLap(int lapCount)
    {
        testDataUtil.AddMonster(m => m.Modifiers = JsonConvert.SerializeObject(new List<ActiveStatsModifier>
        {
            new()
            {
                Active = true,
                Id = 8,
                LapCount = lapCount,
                CurrentLapCount = lapCount,
                Name = "some-name",
                DurationType = "lap",
            },
        }));
    }

    [Given("^a dead monster$")]
    public void GivenADeadMonster()
    {
        testDataUtil.AddMonster(m => m.Dead = new DateTime(2042, 8, 6, 12, 23, 24, DateTimeKind.Utc));
    }

    [Given("^a monster with an item in its inventory$")]
    public void GivenAMonsterWithAnItemInItsInventory()
    {
        testDataUtil.AddMonster();
        testDataUtil.AddItemToMonster();
    }

    [Given(@"^a monster with an item in its inventory and a modifier$")]
    public void GivenAMonsterWithAnItemInItsInventoryAndAModifier()
    {
        testDataUtil.AddMonster(m => m.Modifiers = JsonConvert.SerializeObject(new List<ActiveStatsModifier>
        {
            new()
            {
                Active = true,
                Id = 8,
                Name = "some-name",
                DurationType = "combat",
                CombatCount = 2,
                CurrentCombatCount = 2
            },
        }));
        testDataUtil.AddItemToMonster();
    }
}