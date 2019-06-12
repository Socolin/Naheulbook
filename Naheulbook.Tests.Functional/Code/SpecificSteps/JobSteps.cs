using Naheulbook.TestUtils;
using TechTalk.SpecFlow;

namespace Naheulbook.Tests.Functional.Code.SpecificSteps
{
    [Binding]
    public class JobSteps
    {
        private readonly TestDataUtil _testDataUtil;

        public JobSteps(TestDataUtil testDataUtil)
        {
            _testDataUtil = testDataUtil;
        }

        [Given("a job with all possible data")]
        public void GivenAJobWithAllPossibleData()
        {
            _testDataUtil.AddJobWithAllData();
        }
    }
}