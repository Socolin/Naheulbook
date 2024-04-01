using Naheulbook.Tests.Functional.Code.Utils;
using Naheulbook.TestUtils;
using TechTalk.SpecFlow;

namespace Naheulbook.Tests.Functional.Code.SpecificSteps;

[Binding]
public class StatSteps(TestDataUtil testDataUtil)
{
    [Given(@"(a|\d+) stats?")]
    public void GivenXSkills(string amount)
    {
        for (var i = 0; i < StepArgumentUtil.ParseQuantity(amount); i++)
            testDataUtil.AddStat();
    }
}