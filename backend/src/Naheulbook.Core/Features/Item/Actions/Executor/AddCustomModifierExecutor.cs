﻿using Naheulbook.Core.Features.Character;
using Naheulbook.Core.Notifications;
using Naheulbook.Data.EntityFrameworkCore.Entities;
using Naheulbook.Shared.TransientModels;

namespace Naheulbook.Core.Features.Item.Actions.Executor;

public interface IAddCustomModifierExecutor : IActionExecutor;

public class AddCustomModifierExecutor(ICharacterHistoryUtil characterHistoryUtil) : IAddCustomModifierExecutor
{
    private const string ActionType = "addCustomModifier";

    public Task ExecuteAsync(
        NhbkAction action,
        ActionContext context,
        INotificationSession notificationSession
    )
    {
        if (action.Type != ActionType)
            throw new InvalidActionTypeException(action.Type, ActionType);
        if (action.Data?.Modifier == null)
            throw new InvalidActionDataException(action.Type);

        var modifier = action.Data.Modifier;
        var characterModifier = new CharacterModifierEntity
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
            Values = modifier.Values.Select(v => new CharacterModifierValueEntity
            {
                Type = v.Type,
                StatName = v.Stat,
                Value = v.Value,
            }).ToList(),
        };

        context.TargetCharacter.Modifiers.Add(characterModifier);

        context.TargetCharacter.AddHistoryEntry(characterHistoryUtil.CreateLogAddModifier(context.TargetCharacter, characterModifier));
        notificationSession.NotifyCharacterAddModifier(context.TargetCharacter.Id, characterModifier, true);

        return Task.CompletedTask;
    }
}