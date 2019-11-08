using System;
using System.Collections.Generic;
using Naheulbook.Data.Models;
using Naheulbook.Shared.TransientModels;
using Naheulbook.TestUtils;
using Newtonsoft.Json;
using TechTalk.SpecFlow;

namespace Naheulbook.Tests.Functional.Code.SpecificSteps
{
    [Binding]
    public class MonsterSteps
    {
        private readonly TestDataUtil _testDataUtil;

        public MonsterSteps(TestDataUtil testDataUtil)
        {
            _testDataUtil = testDataUtil;
        }

        [Given("a monster category type")]
        public void GivenAMonsterCategoryType()
        {
            _testDataUtil.AddMonsterType();
        }

        [Given("a monster category")]
        public void GivenAMonsterCategory()
        {
            _testDataUtil.AddMonsterCategory();
        }

        [Given("a monster trait")]
        public void GivenAMonsterTrait()
        {
            _testDataUtil.AddMonsterTrait();
        }

        [Given("a monster template")]
        public void GivenAMonsterTemplate()
        {
            if (!_testDataUtil.Contains<MonsterType>())
                _testDataUtil.AddMonsterType();
            if (!_testDataUtil.Contains<MonsterCategory>())
                _testDataUtil.AddMonsterCategory();
            _testDataUtil.AddMonsterTemplate();
        }

        [Given("a monster template with inventory")]
        public void GivenAMonsterTemplateWithLocationAndInventory()
        {
            if (!_testDataUtil.Contains<MonsterType>())
                _testDataUtil.AddMonsterType();
            if (!_testDataUtil.Contains<MonsterCategory>())
                _testDataUtil.AddMonsterCategory();

            _testDataUtil.AddItemTemplateSection().AddItemTemplateCategory().AddItemTemplate();
            _testDataUtil.AddMonsterTemplate(m =>
            {
                m.Items = new List<MonsterTemplateSimpleInventory>
                {
                    new MonsterTemplateSimpleInventory
                    {
                        Chance = 0.5f,
                        ItemTemplate = _testDataUtil.GetLast<ItemTemplate>(),
                        MinCount = 1,
                        MaxCount = 3
                    }
                };
            });
        }

        [Given("a monster")]
        public void GivenAMonster()
        {
            _testDataUtil.AddMonster();
        }

        [Given(@"a monster with a modifier active for (\d) laps?")]
        public void GivenAMonsterWithAModifierActiveForXLap(int lapCount)
        {
            _testDataUtil.AddMonster(m => m.Modifiers = JsonConvert.SerializeObject(new List<ActiveStatsModifier>
            {
                new ActiveStatsModifier
                {
                    Active = true,
                    Id = 8,
                    LapCount = lapCount,
                    CurrentLapCount = lapCount,
                    Name = "some-name",
                    DurationType = "lap"
                }
            }));
        }

        [Given("a dead monster")]
        public void GivenADeadMonster()
        {
            _testDataUtil.AddMonster(m => m.Dead = new DateTime(2042, 8, 6, 12, 23, 24, DateTimeKind.Utc));
        }

        [Given("a monster with an item in its inventory")]
        public void GivenAMonsterWithAnItemInItsInventory()
        {
            _testDataUtil.AddMonster();
            _testDataUtil.AddItem(_testDataUtil.GetLast<Monster>());
        }
    }
}