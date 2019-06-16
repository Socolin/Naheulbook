using System.Collections.Generic;
using Naheulbook.Data.Models;
using Naheulbook.Tests.Functional.Code.Extensions.ScenarioContextExtensions;
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

        [Given("a character")]
        public void GivenACharacter()
        {
            if (_testDataUtil.Contains<Job>())
                _testDataUtil.AddCharacter(_scenarioContext.GetUserId(), c =>
                {
                    c.Jobs = new List<CharacterJob>
                    {
                        new CharacterJob {JobId = _testDataUtil.GetLast<Job>().Id}
                    };
                });
            else
                _testDataUtil.AddCharacter(_scenarioContext.GetUserId());
        }
    }
}