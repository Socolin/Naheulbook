using Naheulbook.Requests.Requests;
using Naheulbook.Shared.TransientModels;

namespace Naheulbook.Core.Features.Group;

public interface IDurationChangeMapper
{
    FighterDurationChanges ToFighterDurationChanges(PostGroupUpdateDurationsRequest request);
    IDurationChange ToDurationChange(DurationChangeRequest request);
}

public class DurationChangeMapper : IDurationChangeMapper
{
    public FighterDurationChanges ToFighterDurationChanges(PostGroupUpdateDurationsRequest request)
    {
        if (request.MonsterId.HasValue)
            return new MonsterUpdateDuration {MonsterId = request.MonsterId.Value, Changes = request.Changes.Select(ToDurationChange).ToList()};
        if (request.CharacterId.HasValue)
            return new CharacterUpdateDuration {CharacterId = request.CharacterId.Value, Changes = request.Changes.Select(ToDurationChange).ToList()};
        throw new NotSupportedException("Either MonsterId or CharacterId should be set");
    }

    public IDurationChange ToDurationChange(DurationChangeRequest request)
    {
        switch (request.Type)
        {
            case ItemModifierDurationChange.TypeValue:
                if (!request.ItemId.HasValue)
                    throw new NotSupportedException($"Missing itemId when converting DurationChange of type `{request.Type}`");
                return new ItemModifierDurationChange {ItemId = request.ItemId.Value, Modifier = request.Modifier!};
            case ItemLifetimeDurationChange.TypeValue:
                if (!request.ItemId.HasValue)
                    throw new NotSupportedException($"Missing itemId when converting DurationChange of type `{request.Type}`");
                return new ItemLifetimeDurationChange {ItemId = request.ItemId.Value, LifeTime = request.LifeTime!};
            case ModifierDurationChange.TypeValue:
                return new ModifierDurationChange {Modifier = request.Modifier!};
            case SpecialAttackDurationChange.TypeValue:
                if (!request.SpecialAttackId.HasValue)
                    throw new NotSupportedException($"Missing specialAttackId when converting DurationChange of type `{request.Type}`");
                return new SpecialAttackDurationChange {SpecialAttackId = request.SpecialAttackId.Value, Cooldowns = request.Cooldowns ?? []};
            default:
                throw new NotSupportedException($"Invalid ChangeDurationType `{request.Type}`");
        }
    }
}
