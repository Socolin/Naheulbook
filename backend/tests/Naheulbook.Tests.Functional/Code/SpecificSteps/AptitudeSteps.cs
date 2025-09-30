using Naheulbook.TestUtils;
using Reqnroll;

namespace Naheulbook.Tests.Functional.Code.SpecificSteps;

[Binding]
public class AptitudeSteps(TestDataUtil testDataUtil)
{
    [Given("an aptitude group")]
    public void GivenAnAptitudeGroup()
    {
        testDataUtil.AddAptitudeGroup();
    }

    [Given("an aptitude")]
    public void GivenAnAptitude()
    {
        testDataUtil.AddAptitude();
    }
}