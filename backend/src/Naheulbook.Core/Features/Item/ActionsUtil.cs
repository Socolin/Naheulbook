using Naheulbook.Core.Features.Item.Actions;
using Naheulbook.Core.Features.Item.Actions.Executor;
using Naheulbook.Core.Notifications;
using Naheulbook.Shared.TransientModels;

namespace Naheulbook.Core.Features.Item;

public interface IActionsUtil
{
    Task ExecuteActionAsync(NhbkAction action, ActionContext context, INotificationSession notificationSession, CancellationToken cancellationToken = default);
}

public class ActionsUtil(
    IAddItemExecutor addItemExecutor,
    IRemoveItemExecutor removeItemExecutor,
    IAddCustomModifierExecutor addCustomModifierExecutor,
    IAddEffectExecutor addEffectExecutor,
    IAddEvExecutor addEvExecutor,
    IAddEaExecutor addEaExecutor
) : IActionsUtil
{
    public Task ExecuteActionAsync(
        NhbkAction action,
        ActionContext context,
        INotificationSession notificationSession,
        CancellationToken cancellationToken = default
    )
    {
        switch (action.Type)
        {
            case "addEv":
                return addEvExecutor.ExecuteAsync(action, context, notificationSession, cancellationToken);
            case "addEa":
                return addEaExecutor.ExecuteAsync(action, context, notificationSession, cancellationToken);
            case "addItem":
                return addItemExecutor.ExecuteAsync(action, context, notificationSession, cancellationToken);
            case "removeItem":
                return removeItemExecutor.ExecuteAsync(action, context, notificationSession, cancellationToken);
            case "addEffect":
                return addEffectExecutor.ExecuteAsync(action, context, notificationSession, cancellationToken);
            case "addCustomModifier":
                return addCustomModifierExecutor.ExecuteAsync(action, context, notificationSession, cancellationToken);
            default:
                return Task.CompletedTask;
        }
    }
}