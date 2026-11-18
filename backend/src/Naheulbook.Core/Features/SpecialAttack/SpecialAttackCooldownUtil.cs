using Naheulbook.Shared.TransientModels;

namespace Naheulbook.Core.Features.SpecialAttack;

public interface ISpecialAttackCooldownUtil
{
    /// <summary>
    /// A special attack is available when there is no running cooldown from a failure, and when the number of running cooldowns
    /// is lower than the number of uses allowed per cooldown period ("twice per day").
    /// </summary>
    bool IsAvailable(SpecialAttackData specialAttack, CharacterSpecialAttackData state);

    /// <summary>
    /// Count the success and start the matching cooldown (the failure one on failure). Nothing is started when the special
    /// attack has no cooldown (no usage limit) or, on failure, when it cannot fail.
    /// </summary>
    void RegisterUse(SpecialAttackData specialAttack, CharacterSpecialAttackData state, bool success);
}

public class SpecialAttackCooldownUtil : ISpecialAttackCooldownUtil
{
    public bool IsAvailable(SpecialAttackData specialAttack, CharacterSpecialAttackData state)
    {
        if (state.Cooldowns.Any(c => c.Failure))
            return false;

        return state.Cooldowns.Count < (specialAttack.UsesPerCooldown ?? 1);
    }

    public void RegisterUse(SpecialAttackData specialAttack, CharacterSpecialAttackData state, bool success)
    {
        if (success)
            state.SuccessCount++;

        var duration = success ? specialAttack.Cooldown : specialAttack.FailureCooldown;
        if (duration == null)
            return;

        state.Cooldowns.Add(new SpecialAttackCooldown
        {
            DurationType = duration.DurationType,
            CombatCount = duration.CombatCount,
            LapCount = duration.LapCount,
            TimeDuration = duration.TimeDuration,
            Duration = duration.Duration,
            CurrentCombatCount = duration.CombatCount,
            CurrentLapCount = duration.LapCount,
            CurrentTimeDuration = duration.TimeDuration,
            Failure = !success,
        });
    }
}
