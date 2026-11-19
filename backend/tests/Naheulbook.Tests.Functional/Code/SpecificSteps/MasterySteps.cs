using Naheulbook.TestUtils;
using Reqnroll;

namespace Naheulbook.Tests.Functional.Code.SpecificSteps;

[Binding]
public class MasterySteps(TestDataUtil testDataUtil)
{
    [Given("a mastery group")]
    public void GivenAMasteryGroup()
    {
        testDataUtil.AddMasteryGroup();
    }

    [Given("a mastery")]
    public void GivenAMastery()
    {
        testDataUtil.AddMastery();
    }

}
