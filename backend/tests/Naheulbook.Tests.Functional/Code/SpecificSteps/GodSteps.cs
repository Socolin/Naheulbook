using Naheulbook.TestUtils;
using TechTalk.SpecFlow;

namespace Naheulbook.Tests.Functional.Code.SpecificSteps;

[Binding]
public class GodSteps(TestDataUtil testDataUtil)
{
    [Given("a god")]
    public void GivenAGod()
    {
        testDataUtil.AddGod();
    }
}