using System.Collections.Generic;
using Naheulbook.Data.Models;
using Naheulbook.Tests.Functional.Code.Extensions.ScenarioContextExtensions;
using Naheulbook.Tests.Functional.Code.Utils;
using Naheulbook.TestUtils;
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
            if (!_testDataUtil.Contains<Origin>())
                _testDataUtil.AddOrigin();
            if (_testDataUtil.Contains<Job>())
            {
                for (var i = 0; i < StepArgumentUtil.ParseQuantity(amount); i++)
                {
                    _testDataUtil.AddCharacter(_scenarioContext.GetUserId(), c =>
                    {
                        c.Jobs = new List<CharacterJob>
                        {
                            new CharacterJob {JobId = _testDataUtil.GetLast<Job>().Id}
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
            _testDataUtil.AddItem(_testDataUtil.GetLast<Character>());
            _testDataUtil.GetLast<Character>().Items = new List<Item>
            {
                _testDataUtil.GetLast<Item>()
            };
            _testDataUtil.SaveChanges();
        }

        [Given(@"an item in the loot")]
        public void GivenAnItemInTheLoot()
        {
            if (!_testDataUtil.Contains<ItemTemplate>())
                _testDataUtil.AddItemTemplateSection().AddItemTemplateCategory().AddItemTemplate();
            _testDataUtil.AddItem(_testDataUtil.GetLast<Loot>());
            _testDataUtil.SaveChanges();
        }

        [Given("that the (.+) character is a member of the group")]
        public void GivenThatXTheCharacterIsAMemberOfTheGroup(string indexString)
        {
            var character = _testDataUtil.Get<Character>(StepArgumentUtil.ParseIndex(indexString));
            character.GroupId = _testDataUtil.GetLast<Group>().Id;
            _testDataUtil.SaveChanges();
        }

        [Given("that the character is a member of the group")]
        public void GivenThatTheCharacterIsAMemberOfTheGroup()
        {
            var character = _testDataUtil.GetLast<Character>();
            character.GroupId = _testDataUtil.GetLast<Group>().Id;
            _testDataUtil.SaveChanges();
        }

        [Given("a character history entry")]
        public void GivenACharacterHistoryEntry()
        {
            _testDataUtil.AddCharacterHistoryEntry();
        }
    }
}