using AwesomeAssertions;
using Naheulbook.Core.Features.SpecialAttack;
using Naheulbook.Shared.TransientModels;
using NUnit.Framework;

namespace Naheulbook.Core.Tests.Unit.Features.SpecialAttack;

[TestFixture]
[TestOf(typeof(SpecialAttackCooldownUtil))]
public class SpecialAttackCooldownUtilTests
{
    private SpecialAttackCooldownUtil _util;

    [SetUp]
    public void SetUp()
    {
        _util = new SpecialAttackCooldownUtil();
    }

    [Test]
    public void IsAvailable_WithoutRunningCooldown_ReturnsTrue()
    {
        var actual = _util.IsAvailable(new SpecialAttackData(), new CharacterSpecialAttackData());

        actual.Should().BeTrue();
    }

    [Test]
    public void IsAvailable_WithARunningCooldown_ReturnsFalse()
    {
        var state = new CharacterSpecialAttackData {Cooldowns = [new SpecialAttackCooldown {DurationType = "time", CurrentTimeDuration = 10}]};

        var actual = _util.IsAvailable(new SpecialAttackData(), state);

        actual.Should().BeFalse();
    }

    [Test]
    public void IsAvailable_WithLessRunningCooldownsThanUsesPerCooldown_ReturnsTrue()
    {
        var state = new CharacterSpecialAttackData {Cooldowns = [new SpecialAttackCooldown {DurationType = "time", CurrentTimeDuration = 10}]};

        var actual = _util.IsAvailable(new SpecialAttackData {UsesPerCooldown = 2}, state);

        actual.Should().BeTrue();
    }

    [Test]
    public void IsAvailable_WithARunningFailureCooldown_ReturnsFalseEvenWithRemainingUses()
    {
        var state = new CharacterSpecialAttackData {Cooldowns = [new SpecialAttackCooldown {DurationType = "time", CurrentTimeDuration = 10, Failure = true}]};

        var actual = _util.IsAvailable(new SpecialAttackData {UsesPerCooldown = 2}, state);

        actual.Should().BeFalse();
    }

    [Test]
    public void RegisterUse_OnSuccess_IncrementSuccessCountAndStartTheCooldown()
    {
        var specialAttack = new SpecialAttackData
        {
            Cooldown = new Durable {DurationType = "time", TimeDuration = 3600},
            FailureCooldown = new Durable {DurationType = "time", TimeDuration = 1800},
        };
        var state = new CharacterSpecialAttackData {SuccessCount = 2};

        _util.RegisterUse(specialAttack, state, true);

        state.Should().BeEquivalentTo(new CharacterSpecialAttackData
        {
            SuccessCount = 3,
            Cooldowns = [new SpecialAttackCooldown {DurationType = "time", TimeDuration = 3600, CurrentTimeDuration = 3600, Failure = false}],
        });
    }

    [Test]
    public void RegisterUse_OnFailure_StartTheFailureCooldownWithoutIncrementingSuccessCount()
    {
        var specialAttack = new SpecialAttackData
        {
            Cooldown = new Durable {DurationType = "combat", CombatCount = 1},
            FailureCooldown = new Durable {DurationType = "combat", CombatCount = 1},
        };
        var state = new CharacterSpecialAttackData {SuccessCount = 2};

        _util.RegisterUse(specialAttack, state, false);

        state.Should().BeEquivalentTo(new CharacterSpecialAttackData
        {
            SuccessCount = 2,
            Cooldowns = [new SpecialAttackCooldown {DurationType = "combat", CombatCount = 1, CurrentCombatCount = 1, Failure = true}],
        });
    }

    [Test]
    public void RegisterUse_WithoutCooldown_OnlyCountTheSuccess()
    {
        var state = new CharacterSpecialAttackData();

        _util.RegisterUse(new SpecialAttackData(), state, true);

        state.SuccessCount.Should().Be(1);
        state.Cooldowns.Should().BeEmpty();
    }
}
