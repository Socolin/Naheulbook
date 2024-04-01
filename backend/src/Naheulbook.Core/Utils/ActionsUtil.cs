using System.Threading.Tasks;
using Naheulbook.Core.Actions;
using Naheulbook.Core.Actions.Executor;
using Naheulbook.Core.Notifications;
using Naheulbook.Shared.TransientModels;

namespace Naheulbook.Core.Utils;

public interface IActionsUtil
{
    Task ExecuteActionAsync(NhbkAction action, ActionContext context, INotificationSession notificationSession);
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
        INotificationSession notificationSession
    )
    {
        switch (action.Type)
        {
            case "addEv":
                return addEvExecutor.ExecuteAsync(action, context, notificationSession);
            case "addEa":
                return addEaExecutor.ExecuteAsync(action, context, notificationSession);
            case "addItem":
                return addItemExecutor.ExecuteAsync(action, context, notificationSession);
            case "removeItem":
                return removeItemExecutor.ExecuteAsync(action, context, notificationSession);
            case "addEffect":
                return addEffectExecutor.ExecuteAsync(action, context, notificationSession);
            case "addCustomModifier":
                return addCustomModifierExecutor.ExecuteAsync(action, context, notificationSession);
            default:
                return Task.CompletedTask;
        }
    }
}