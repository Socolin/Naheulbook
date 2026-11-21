using Naheulbook.Data.EntityFrameworkCore.Entities;
using Naheulbook.TestUtils;
using Reqnroll;

namespace Naheulbook.Tests.Functional.Code.SpecificSteps;

[Binding]
public class EntropySteps(TestDataUtil testDataUtil)
{
    [Given("a mankdebol calamity")]
    public void GivenAMankdebolCalamity()
    {
        testDataUtil.AddMankdebolCalamity();
    }

    [Given("a debilibeuk blessing")]
    public void GivenADebilibeukBlessing()
    {
        testDataUtil.AddDebilibeukBlessing();
    }

    [Given(@"^that the group has (\d+) mankdebol points$")]
    public void GivenThatTheGroupHasMankdebolPoints(int points)
    {
        testDataUtil.GetLast<GroupEntity>().Data = $$"""{"mankdebol": {{points}}}""";
        testDataUtil.SaveChanges();
    }
}
