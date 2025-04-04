using Naheulbook.TestUtils;
using Reqnroll;

namespace Naheulbook.Tests.Functional.Code.SpecificSteps;

[Binding]
public class MiscSteps(TestDataUtil testDataUtil)
{
    [Given("^a calendar entry$")]
    public void GivenACalendarEntry()
    {
        testDataUtil.AddCalendarEntry();
    }
}