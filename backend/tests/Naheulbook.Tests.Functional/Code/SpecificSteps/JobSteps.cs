using Naheulbook.Tests.Functional.Code.Utils;
using Naheulbook.TestUtils;
using TechTalk.SpecFlow;

namespace Naheulbook.Tests.Functional.Code.SpecificSteps;

[Binding]
public class JobSteps(TestDataUtil testDataUtil)
{
    [Given(@"(a) job")]
    [Given(@"(.*) jobs")]
    public void GivenAJob(string amount)
    {
        for (var i = 0; i < StepArgumentUtil.ParseQuantity(amount); i++)
            testDataUtil.AddJob();
    }

    [Given(@"(a) speciality")]
    [Given(@"(.*) specialities")]
    public void GivenSpecialities(string amount)
    {
        for (var i = 0; i < StepArgumentUtil.ParseQuantity(amount); i++)
            testDataUtil.AddSpeciality();
    }

    [Given("a job with all possible data")]
    public void GivenAJobWithAllPossibleData()
    {
        testDataUtil.AddJobWithAllData();
    }
}