using Naheulbook.Tests.Functional.Code.Utils;
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

        [Given(@"(a|\d+) locations?")]
        public void GivenXLocation(string amount)
        {
            for (var i = 0; i < StepArgumentUtil.ParseQuantity(amount); i++)
                _testDataUtil.AddLocation();
        }
    }
}