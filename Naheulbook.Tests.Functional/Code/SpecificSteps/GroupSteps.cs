using Naheulbook.Tests.Functional.Code.Extensions.ScenarioContextExtensions;
using Naheulbook.TestUtils;
using TechTalk.SpecFlow;

namespace Naheulbook.Tests.Functional.Code.SpecificSteps
{
    [Binding]
    public class GroupSteps
    {
        private readonly TestDataUtil _testDataUtil;
        private readonly ScenarioContext _scenarioContext;

        public GroupSteps(TestDataUtil testDataUtil, ScenarioContext scenarioContext)
        {
            _testDataUtil = testDataUtil;
            _scenarioContext = scenarioContext;
        }

        [Given("a group")]
        public void GivenAGroup()
        {
            _testDataUtil.AddLocation();

            _testDataUtil.AddGroup(_scenarioContext.GetUserId());
        }
    }
}