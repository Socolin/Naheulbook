using Naheulbook.Tests.Functional.Code.Utils;
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

        [Given(@"(a) job")]
        [Given(@"(.*) jobs")]
        public void GivenAJob(string amount)
        {
            for (var i = 0; i < StepArgumentUtil.ParseQuantity(amount); i++)
                _testDataUtil.AddJob();
        }

        [Given(@"(a) speciality")]
        [Given(@"(.*) specialities")]
        public void GivenSpecialities(string amount)
        {
            for (var i = 0; i < StepArgumentUtil.ParseQuantity(amount); i++)
                _testDataUtil.AddSpeciality();
        }

        [Given("a job with all possible data")]
        public void GivenAJobWithAllPossibleData()
        {
            _testDataUtil.AddJobWithAllData();
        }
    }
}