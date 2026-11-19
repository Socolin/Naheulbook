using Naheulbook.TestUtils;
using Reqnroll;

namespace Naheulbook.Tests.Functional.Code.SpecificSteps;

[Binding]
public class TalentSteps(TestDataUtil testDataUtil)
{
    [Given("a talent group")]
    public void GivenATalentGroup()
    {
        testDataUtil.AddTalentGroup();
    }

    [Given("a talent")]
    public void GivenATalent()
    {
        testDataUtil.AddTalent();
    }
}
