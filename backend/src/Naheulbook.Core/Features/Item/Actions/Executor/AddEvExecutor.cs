using Naheulbook.Core.Features.Character;
using Naheulbook.Core.Notifications;
using Naheulbook.Shared.TransientModels;

namespace Naheulbook.Core.Features.Item.Actions.Executor;

public interface IAddEvExecutor : IActionExecutor;

public class AddEvExecutor(ICharacterHistoryUtil characterHistoryUtil) : IAddEvExecutor
{
    private const string ActionType = "addEv";

    public Task ExecuteAsync(
        NhbkAction action,
        ActionContext context,
        INotificationSession notificationSession
    )
    {
        if (action.Type != ActionType)
            throw new InvalidActionTypeException(action.Type, ActionType);
        if (action.Data == null)
            throw new InvalidActionDataException(action.Type);
        if (!action.Data.Ev.HasValue)
            throw new InvalidActionDataException(action.Type);

        var oldEv = context.TargetCharacter.Ev ?? 0;
        var newEv = oldEv + action.Data.Ev.Value;

        context.TargetCharacter.AddHistoryEntry(characterHistoryUtil.CreateLogChangeEv(context.TargetCharacter, oldEv, newEv));
        context.TargetCharacter.Ev = newEv;
        notificationSession.NotifyCharacterChangeEv(context.TargetCharacter);

        return Task.CompletedTask;
    }
}