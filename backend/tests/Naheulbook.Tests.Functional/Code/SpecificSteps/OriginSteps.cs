using Naheulbook.TestUtils;
using Reqnroll;

namespace Naheulbook.Tests.Functional.Code.SpecificSteps;

[Binding]
public class OriginSteps(TestDataUtil testDataUtil)
{
    [Given("^an origin$")]
    public void GivenAnOrigin()
    {
        testDataUtil.AddOrigin();
    }

    [Given("^an origin with random name api configured$")]
    public void GivenAnOriginWithRandomNameApiConfigured()
    {
        testDataUtil.AddOrigin();
        testDataUtil.AddOriginRandomNameUrl();
    }

    [Given("^an origin with all possible data$")]
    public void GivenAnOriginWithAllPossibleData()
    {
        testDataUtil.AddOriginWithAllData();
    }
}