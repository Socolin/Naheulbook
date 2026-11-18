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

    [Given("a hybrid origin which can choose its aptitude table, with an aptitude in the alternative table")]
    public void GivenAHybridOriginWhichCanChooseItsAptitudeTable()
    {
        testDataUtil
            .AddAptitudeGroup(out var originAptitudeGroup)
            .AddAptitudeGroup(out var alternativeAptitudeGroup)
            .AddOrigin(o =>
            {
                o.AptitudeGroupId = originAptitudeGroup.Id;
                o.Data = $$"""{"baseEv": 28, "diceEvLevelUp": 6, "alternativeAptitudeGroupIds": ["{{alternativeAptitudeGroup.Id}}"]}""";
            })
            .AddAptitude();
    }
}
