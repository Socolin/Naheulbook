using System.Collections.Generic;
using Naheulbook.Data.Models;
using Naheulbook.Tests.Functional.Code.Extensions.ScenarioContextExtensions;
using Naheulbook.Tests.Functional.Code.Utils;
using Naheulbook.TestUtils;
using Newtonsoft.Json;
using TechTalk.SpecFlow;

namespace Naheulbook.Tests.Functional.Code.SpecificSteps
{
    [Binding]
    public class CharacterSteps
    {
        private readonly TestDataUtil _testDataUtil;
        private readonly ScenarioContext _scenarioContext;

        public CharacterSteps(
            TestDataUtil testDataUtil,
            ScenarioContext scenarioContext
        )
        {
            _testDataUtil = testDataUtil;
            _scenarioContext = scenarioContext;
        }

        [Given(@"(a|\d+) characters?")]
        public void GivenACharacter(string amount)
        {
            if (!_testDataUtil.Contains<OriginEntity>())
                _testDataUtil.AddOrigin();
            if (_testDataUtil.Contains<JobEntity>())
            {
                for (var i = 0; i < StepArgumentUtil.ParseQuantity(amount); i++)
                {
                    _testDataUtil.AddCharacter(_scenarioContext.GetUserId(), c =>
                    {
                        c.Jobs = new List<CharacterJobEntity>
                        {
                            new CharacterJobEntity {JobId = _testDataUtil.GetLast<JobEntity>().Id}
                        };
                    });
                }
            }
            else
            {
                for (var i = 0; i < StepArgumentUtil.ParseQuantity(amount); i++)
                    _testDataUtil.AddCharacter(_scenarioContext.GetUserId());
            }
        }

        [Given(@"a character modifier")]
        public void GivenACharacterModifier()
        {
            _testDataUtil.AddCharacterModifier();
        }

        [Given(@"an (inactive|active) (non-reusable|reusable) character modifier active for (\d) lap")]
        public void GivenACharacterModifier(string active, string reusable, int lapCount)
        {
            _testDataUtil.AddCharacterModifier(c =>
            {
                c.Reusable = reusable == "reusable";
                c.IsActive = active == "active";
                c.LapCount = lapCount;
                c.CurrentLapCount = lapCount;
            });
        }

        [Given(@"an inactive reusable character modifier that last 2 combat")]
        public void GivenAnInactiveReusableCharacterModifierThatLast2Combat()
        {
            _testDataUtil.AddCharacterModifier(c =>
            {
                c.Reusable = true;
                c.IsActive = false;
                c.CombatCount = 2;
            });
        }

        [Given(@"a character with all possible data")]
        public void GivenACharacterWithAllPossibleData()
        {
            _testDataUtil.AddCharacterWithAllData(_scenarioContext.GetUserId());
        }

        [Given(@"an item based on that item template in the character inventory")]
        public void GivenAnItemBasedOnThatItemTemplateInTheCharacterInventory()
        {
            _testDataUtil.AddItem(_testDataUtil.GetLast<CharacterEntity>());
            _testDataUtil.GetLast<CharacterEntity>().Items = new List<ItemEntity>
            {
                _testDataUtil.GetLast<ItemEntity>()
            };
            _testDataUtil.SaveChanges();
        }

        [Given(@"an item based on that item template in the character inventory with (\d+) charges?")]
        public void GivenAnItemBasedOnThatItemTemplateInTheCharacterInventoryWithXCharge(int chargeCount)
        {
            _testDataUtil.AddItem(_testDataUtil.GetLast<CharacterEntity>(), item =>
            {
                item.Data = JsonConvert.SerializeObject(new {charge = chargeCount});
            });

            _testDataUtil.GetLast<CharacterEntity>().Items = new List<ItemEntity>
            {
                _testDataUtil.GetLast<ItemEntity>()
            };
            _testDataUtil.SaveChanges();
        }

        [Given(@"an item in the loot")]
        public void GivenAnItemInTheLoot()
        {
            if (!_testDataUtil.Contains<ItemTemplateEntity>())
                _testDataUtil.AddItemTemplateSection().AddItemTemplateSubCategory().AddItemTemplate();
            _testDataUtil.AddItem(_testDataUtil.GetLast<LootEntity>());
            _testDataUtil.SaveChanges();
        }

        [Given(@"an item in the character inventory")]
        public void GivenAnItemInTheCharacterInventory()
        {
            if (!_testDataUtil.Contains<ItemTemplateEntity>())
                _testDataUtil.AddItemTemplateSection().AddItemTemplateSubCategory().AddItemTemplate();
            _testDataUtil.AddItem(_testDataUtil.GetLast<CharacterEntity>());
            _testDataUtil.SaveChanges();
        }

        [Given("that the (.+) character is a member of the group")]
        public void GivenThatXTheCharacterIsAMemberOfTheGroup(string indexString)
        {
            var character = _testDataUtil.Get<CharacterEntity>(StepArgumentUtil.ParseIndex(indexString));
            character.GroupId = _testDataUtil.GetLast<GroupEntity>().Id;
            _testDataUtil.SaveChanges();
        }

        [Given("that the character is a member of the group")]
        public void GivenThatTheCharacterIsAMemberOfTheGroup()
        {
            var character = _testDataUtil.GetLast<CharacterEntity>();
            character.GroupId = _testDataUtil.GetLast<GroupEntity>().Id;
            _testDataUtil.SaveChanges();
        }

        [Given("a character history entry")]
        public void GivenACharacterHistoryEntry()
        {
            _testDataUtil.AddCharacterHistoryEntry();
        }
    }
}