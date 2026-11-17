using Naheulbook.Core.Features.Shared;
using Naheulbook.Data.EntityFrameworkCore.Entities;
using Naheulbook.Requests.Requests;
using Naheulbook.Shared.TransientModels;

namespace Naheulbook.Core.Features.Character;

public interface ICharacterModifierMapper
{
    ActiveStatsModifier ToResponse(CharacterModifierEntity characterModifier);
    StatModifierRequest ToResponse(CharacterModifierValueEntity characterModifierValue);
    CharacterModifierEntity ToEntity(AddCharacterModifierRequest request);
}

public class CharacterModifierMapper : ICharacterModifierMapper
{
    public ActiveStatsModifier ToResponse(CharacterModifierEntity characterModifier)
    {
        return new ActiveStatsModifier
        {
            Id = characterModifier.Id,
            Name = characterModifier.Name,
            Active = characterModifier.IsActive,
            Permanent = characterModifier.Permanent,
            Reusable = characterModifier.Reusable,
            Description = characterModifier.Description,
            Type = characterModifier.Type,
            DurationType = characterModifier.DurationType,
            Duration = characterModifier.Duration,
            CombatCount = characterModifier.CombatCount,
            LapCount = characterModifier.LapCount,
            TimeDuration = characterModifier.TimeDuration,
            CurrentCombatCount = characterModifier.CurrentCombatCount,
            CurrentLapCount = characterModifier.CurrentLapCount,
            CurrentTimeDuration = characterModifier.CurrentTimeDuration,
            LapCountDecrement = MapperHelpers.FromJson<LapCountDecrement>(characterModifier.LapCountDecrement),
            Values = characterModifier.Values.OrderBy(v => v.Id).Select(ToResponse).ToList(),
        };
    }

    public StatModifierRequest ToResponse(CharacterModifierValueEntity characterModifierValue)
    {
        return new StatModifierRequest
        {
            Stat = characterModifierValue.StatName,
            Type = characterModifierValue.Type,
            Value = characterModifierValue.Value,
        };
    }

    public CharacterModifierEntity ToEntity(AddCharacterModifierRequest request)
    {
        return new CharacterModifierEntity
        {
            Name = request.Name,
            IsActive = true,
            Permanent = false,
            Reusable = request.Reusable,
            Description = request.Description,
            Type = request.Type,
            DurationType = request.DurationType,
            Duration = request.Duration,
            CombatCount = request.CombatCount,
            LapCount = request.LapCount,
            TimeDuration = request.TimeDuration,
            CurrentCombatCount = request.CombatCount,
            CurrentLapCount = request.LapCount,
            CurrentTimeDuration = request.TimeDuration,
            LapCountDecrement = MapperHelpers.ToJson(request.LapCountDecrement),
            Values = request.Values.Select(v => new CharacterModifierValueEntity
            {
                StatName = v.Stat,
                Type = v.Type,
                Value = v.Value,
            }).ToList(),
        };
    }
}
