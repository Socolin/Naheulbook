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

        [Given(@"a character with all possible data")]
        public void GivenACharacterWithAllPossibleData()
        {
            _testDataUtil.AddCharacterWithAllData(_scenarioContext.GetUserId());
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