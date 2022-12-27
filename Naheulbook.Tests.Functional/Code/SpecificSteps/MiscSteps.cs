using Naheulbook.TestUtils;
using TechTalk.SpecFlow;

namespace Naheulbook.Tests.Functional.Code.SpecificSteps;

[Binding]
public class MiscSteps
{
    private readonly TestDataUtil _testDataUtil;

    public MiscSteps(TestDataUtil testDataUtil)
    {
        _testDataUtil = testDataUtil;
    }

    [Given("a calendar entry")]
    public void GivenACalendarEntry()
    {
        _testDataUtil.AddCalendarEntry();
    }
}