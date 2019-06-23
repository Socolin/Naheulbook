using Naheulbook.TestUtils;
using TechTalk.SpecFlow;

namespace Naheulbook.Tests.Functional.Code.Steps
{
    [Binding]
    public class DatabaseSteps
    {
        private readonly TestDataUtil _testDataUtil;

        public DatabaseSteps(TestDataUtil testDataUtil)
        {
            _testDataUtil = testDataUtil;
        }

        [Given("a clean database")]
        public void GivenACleanDatabase()
        {
            _testDataUtil.Cleanup();
        }
    }
}