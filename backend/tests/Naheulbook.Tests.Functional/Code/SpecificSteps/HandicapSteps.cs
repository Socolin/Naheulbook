using Naheulbook.TestUtils;
using Reqnroll;

namespace Naheulbook.Tests.Functional.Code.SpecificSteps;

[Binding]
public class HandicapSteps(TestDataUtil testDataUtil)
{
    [Given("a handicap")]
    public void GivenAHandicap()
    {
        testDataUtil.AddHandicap();
    }
}
