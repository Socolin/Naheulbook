using Naheulbook.TestUtils;
using Reqnroll;

namespace Naheulbook.Tests.Functional.Code.SpecificSteps;

[Binding]
public class SpecialAttackSteps(TestDataUtil testDataUtil)
{
    [Given("a special attack group")]
    public void GivenASpecialAttackGroup()
    {
        testDataUtil.AddSpecialAttackGroup();
    }

    [Given("a special attack")]
    public void GivenASpecialAttack()
    {
        testDataUtil.AddSpecialAttack();
    }
}
