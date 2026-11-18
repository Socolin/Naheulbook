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

    [Given("a special attack usable once per hour, with a 30 minutes cooldown on failure, giving 2 XP")]
    public void GivenASpecialAttackUsableOncePerHour()
    {
        testDataUtil.AddSpecialAttack(s => s.Data = """
                                                    {
                                                      "cooldown": {"durationType": "time", "timeDuration": 3600},
                                                      "failureCooldown": {"durationType": "time", "timeDuration": 1800},
                                                      "xp": 2
                                                    }
                                                    """);
    }

    [Given("that the character knows the special attack")]
    public void GivenThatTheCharacterKnowsTheSpecialAttack()
    {
        testDataUtil.AddCharacterSpecialAttack();
    }

    [Given("that the character knows the special attack, with a running cooldown")]
    public void GivenThatTheCharacterKnowsTheSpecialAttackWithARunningCooldown()
    {
        testDataUtil.AddCharacterSpecialAttack(c => c.Data = """
                                                             {
                                                               "successCount": 1,
                                                               "cooldowns": [{"durationType": "time", "timeDuration": 3600, "currentTimeDuration": 600, "failure": false}]
                                                             }
                                                             """);
    }
}
