using Naheulbook.TestUtils;
using TechTalk.SpecFlow;

namespace Naheulbook.Tests.Functional.Code.SpecificSteps
{
    [Binding]
    public class GodSteps
    {
        private readonly TestDataUtil _testDataUtil;

        public GodSteps(TestDataUtil testDataUtil)
        {
            _testDataUtil = testDataUtil;
        }

        [Given("a god")]
        public void GivenAGod()
        {
            _testDataUtil.AddGod();
        }
    }
}