using Naheulbook.TestUtils;
using Reqnroll;

namespace Naheulbook.Tests.Functional.Code.SpecificSteps;

[Binding]
public class SpellSteps(TestDataUtil testDataUtil)
{
    [Given("a spell book")]
    public void GivenASpellBook()
    {
        testDataUtil.AddSpellBook();
    }

    [Given("a spell")]
    public void GivenASpell()
    {
        testDataUtil.AddSpell();
    }
}
