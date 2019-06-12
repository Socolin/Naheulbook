using Naheulbook.TestUtils;
using TechTalk.SpecFlow;

namespace Naheulbook.Tests.Functional.Code.SpecificSteps
{
    [Binding]
    public class OriginSteps
    {
        private readonly TestDataUtil _testDataUtil;

        public OriginSteps(TestDataUtil testDataUtil)
        {
            _testDataUtil = testDataUtil;
        }

        [Given("a origin with all possible data")]
        public void GivenAJobWithAllPossibleData()
        {
            _testDataUtil.AddOriginWithAllData();
        }
    }
}