using System.Linq;
using System.Threading.Tasks;
using Naheulbook.Core.Exceptions;
using Naheulbook.Core.Notifications;
using Naheulbook.Core.Utils;
using Naheulbook.Data.Models;
using Naheulbook.Shared.TransientModels;

namespace Naheulbook.Core.Actions.Executor;

public interface IAddEffectExecutor : IActionExecutor;

public class AddEffectExecutor(ICharacterHistoryUtil characterHistoryUtil) : IAddEffectExecutor
{
    private const string ActionType = "addEffect";

    public async Task ExecuteAsync(
        NhbkAction action,
        ActionContext context,
        INotificationSession notificationSession
    )
    {
        if (action.Type != ActionType)
            throw new InvalidActionTypeException(action.Type, ActionType);
        if (action.Data == null)
            throw new InvalidActionDataException(action.Type);
        if (!action.Data.EffectId.HasValue)
            throw new InvalidActionDataException(action.Type);
        if (action.Data.EffectData == null)
            throw new InvalidActionDataException(action.Type);

        var effect = await context.UnitOfWork.Effects.GetWithEffectWithModifiersAsync(action.Data.EffectId.Value);
        if (effect == null)
            throw new EffectNotFoundException();

        var combatCount = effect.CombatCount;
        var timeDuration = effect.TimeDuration;
        var duration = effect.Duration;
        var lapCount = effect.LapCount;

        var customDurationType = action.Data.EffectData.Value<string?>("durationType");
        if (!string.IsNullOrEmpty(customDurationType))
        {
            timeDuration = null;
            duration = null;
            combatCount = null;
            lapCount = null;
            switch (customDurationType)
            {
                case "combat":
                    combatCount = action.Data.EffectData.Value<int?>("combatCount");
                    break;
                case "time":
                    timeDuration = action.Data.EffectData.Value<int?>("timeDuration");
                    break;
                case "custom":
                    duration = action.Data.EffectData.Value<string>("duration");
                    break;
                case "lap":
                    lapCount = action.Data.EffectData.Value<int?>("lapCount");
                    break;
                case "forever":
                    break;
                default:
                    throw new InvalidCustomDurationActionException(customDurationType);
            }
        }



        var characterModifier = new CharacterModifierEntity
        {
            Name = effect.Name,
            Permanent = false,
            DurationType = customDurationType ?? effect.DurationType,
            Duration = duration,
            Type = effect.SubCategory.Name,
            Description = effect.Description,
            Reusable = false,
            IsActive = true,
            CombatCount = combatCount,
            CurrentCombatCount = combatCount,
            TimeDuration = timeDuration,
            CurrentTimeDuration = timeDuration,
            LapCount = lapCount,
            CurrentLapCount = lapCount,
            Values = effect.Modifiers.Select(v => new CharacterModifierValueEntity
            {
                Type = v.Type,
                StatName = v.StatName,
                Value = v.Value,
            }).ToList(),
        };

        context.TargetCharacter.Modifiers.Add(characterModifier);

        context.TargetCharacter.AddHistoryEntry(characterHistoryUtil.CreateLogAddModifier(context.TargetCharacter, characterModifier));
        notificationSession.NotifyCharacterAddModifier(context.TargetCharacter.Id, characterModifier, true);
    }
}