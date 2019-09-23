using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Naheulbook.Core.Exceptions;
using Naheulbook.Core.Notifications;
using Naheulbook.Core.Utils;
using Naheulbook.Data.Models;
using Naheulbook.Shared.TransientModels;

namespace Naheulbook.Core.Actions.Executor
{
    public interface IAddEffectExecutor : IActionExecutor
    {
    }

    public class AddEffectExecutor : IAddEffectExecutor
    {
        private const string ActionType = "addEffect";
        private readonly ICharacterHistoryUtil _characterHistoryUtil;

        public AddEffectExecutor(ICharacterHistoryUtil characterHistoryUtil)
        {
            _characterHistoryUtil = characterHistoryUtil;
        }

        public async Task ExecuteAsync(
            NhbkAction action,
            ActionContext context,
            INotificationSession notificationSession
        )
        {
            if (action.Type != ActionType)
                throw new InvalidActionTypeException(action.Type, ActionType);
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
            var durationType = action.Data.EffectData.Value<string>("durationType");

            switch (durationType)
            {
                case "combat":
                    timeDuration = null;
                    duration = null;
                    combatCount = action.Data.EffectData.Value<int?>("combatCount");
                    lapCount = null;
                    break;
                case "time":
                    combatCount = null;
                    duration = null;
                    timeDuration = action.Data.EffectData.Value<int?>("timeDuration");
                    lapCount = null;
                    break;
                case "custom":
                    combatCount = null;
                    duration = action.Data.EffectData.Value<string>("duration");
                    timeDuration = null;
                    lapCount = null;
                    break;
                case "lap":
                    combatCount = null;
                    duration = null;
                    timeDuration = null;
                    lapCount = action.Data.EffectData.Value<int?>("lapCount");
                    break;
                case "forever":
                    combatCount = null;
                    duration = null;
                    timeDuration = null;
                    lapCount = null;
                    break;
            }


            var characterModifier = new CharacterModifier
            {
                Name = effect.Name,
                Permanent = false,
                DurationType = durationType,
                Duration = duration,
                Type = effect.Category.Name,
                Description = effect.Description,
                Reusable = false,
                IsActive = true,
                CombatCount = combatCount,
                CurrentCombatCount = combatCount,
                TimeDuration = timeDuration,
                CurrentTimeDuration = timeDuration,
                LapCount = lapCount,
                CurrentLapCount = lapCount,
                Values = effect.Modifiers.Select(v => new CharacterModifierValue
                {
                    Type = v.Type,
                    StatName = v.StatName,
                    Value = v.Value
                }).ToList()
            };

            if (context.TargetCharacter.Modifiers == null)
            {
                context.TargetCharacter.Modifiers = new List<CharacterModifier>();
            }

            context.TargetCharacter.Modifiers.Add(characterModifier);

            context.TargetCharacter.AddHistoryEntry(_characterHistoryUtil.CreateLogAddModifier(context.TargetCharacter, characterModifier));
            notificationSession.NotifyCharacterAddModifier(context.TargetCharacter.Id, characterModifier);
        }
    }
}