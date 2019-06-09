using Naheulbook.TestUtils;
using TechTalk.SpecFlow;

namespace Naheulbook.Tests.Functional.Code.SpecificSteps
{
    [Binding]
    public class LocationSteps
    {
        private readonly TestDataUtil _testDataUtil;

        public LocationSteps(TestDataUtil testDataUtil)
        {
            _testDataUtil = testDataUtil;
        }

        [Given("a location")]
        public void GivenALocation()
        {
            _testDataUtil.AddLocation();
        }
    }
}