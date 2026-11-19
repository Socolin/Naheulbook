using AwesomeAssertions;
using Naheulbook.Core.Features.Character;
using NUnit.Framework;

namespace Naheulbook.Core.Tests.Unit.Features.Character;

[TestFixture]
[TestOf(typeof(TalentOrMasteryPointRule))]
public class TalentOrMasteryPointRuleTests
{
    [TestCase(1, false)]
    [TestCase(2, false)]
    [TestCase(3, true)]
    [TestCase(4, false)]
    [TestCase(5, true)]
    [TestCase(10, false)]
    [TestCase(19, true)]
    public void GivesPointAtLevel_OnlyOnOddLevelsFrom3(int level, bool expected)
    {
        TalentOrMasteryPointRule.GivesPointAtLevel(level).Should().Be(expected);
    }
}
