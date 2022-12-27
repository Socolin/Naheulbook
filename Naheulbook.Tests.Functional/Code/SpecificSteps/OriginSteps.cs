using Naheulbook.TestUtils;
using TechTalk.SpecFlow;

namespace Naheulbook.Tests.Functional.Code.SpecificSteps;

[Binding]
public class OriginSteps
{
    private readonly TestDataUtil _testDataUtil;

    public OriginSteps(TestDataUtil testDataUtil)
    {
        _testDataUtil = testDataUtil;
    }

    [Given("an origin")]
    public void GivenAnOrigin()
    {
        _testDataUtil.AddOrigin();
    }

    [Given("an origin with random name api configured")]
    public void GivenAnOriginWithRandomNameApiConfigured()
    {
        _testDataUtil.AddOrigin();
        _testDataUtil.AddOriginRandomNameUrl();
    }

    [Given("an origin with all possible data")]
    public void GivenAnOriginWithAllPossibleData()
    {
        _testDataUtil.AddOriginWithAllData();
    }
}