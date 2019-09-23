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
    public interface IAddCustomModifierExecutor : IActionExecutor
    {
    }

    public class AddCustomModifierExecutor : IAddCustomModifierExecutor
    {
        private const string ActionType = "addCustomModifier";
        private readonly ICharacterHistoryUtil _characterHistoryUtil;

        public AddCustomModifierExecutor(ICharacterHistoryUtil characterHistoryUtil)
        {
            _characterHistoryUtil = characterHistoryUtil;
        }

        public Task ExecuteAsync(
            NhbkAction action,
            ActionContext context,
            INotificationSession notificationSession
        )
        {
            if (action.Type != ActionType)
                throw new InvalidActionTypeException(action.Type, ActionType);
            if (action.Data.Modifier == null)
                throw new InvalidActionDataException(action.Type);

            var modifier = action.Data.Modifier;
            var characterModifier = new CharacterModifier
            {
                Name = modifier.Name,
                Permanent = false,
                DurationType = modifier.DurationType,
                Duration = modifier.Duration,
                Type = modifier.Type,
                Description = modifier.Description,
                Reusable = modifier.Reusable,
                IsActive = true,
                CombatCount = modifier.CombatCount,
                CurrentCombatCount = modifier.CombatCount,
                TimeDuration = modifier.TimeDuration,
                CurrentTimeDuration = modifier.TimeDuration,
                LapCount = modifier.LapCount,
                CurrentLapCount = modifier.LapCount,
                Values = modifier.Values.Select(v => new CharacterModifierValue
                {
                    Type = v.Type,
                    StatName = v.Stat,
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

            return Task.CompletedTask;
        }
    }
}