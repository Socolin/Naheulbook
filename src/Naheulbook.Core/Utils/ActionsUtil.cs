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

public class ActionsUtil : IActionsUtil
{
    private readonly IAddItemExecutor _addItemExecutor;
    private readonly IRemoveItemExecutor _removeItemExecutor;
    private readonly IAddCustomModifierExecutor _addCustomModifierExecutor;
    private readonly IAddEffectExecutor _addEffectExecutor;
    private readonly IAddEvExecutor _addEvExecutor;
    private readonly IAddEaExecutor _addEaExecutor;

    public ActionsUtil(
        IAddItemExecutor addItemExecutor,
        IRemoveItemExecutor removeItemExecutor,
        IAddCustomModifierExecutor addCustomModifierExecutor,
        IAddEffectExecutor addEffectExecutor,
        IAddEvExecutor addEvExecutor,
        IAddEaExecutor addEaExecutor
    )
    {
        _addItemExecutor = addItemExecutor;
        _removeItemExecutor = removeItemExecutor;
        _addCustomModifierExecutor = addCustomModifierExecutor;
        _addEffectExecutor = addEffectExecutor;
        _addEvExecutor = addEvExecutor;
        _addEaExecutor = addEaExecutor;
    }

    public Task ExecuteActionAsync(
        NhbkAction action,
        ActionContext context,
        INotificationSession notificationSession
    )
    {
        switch (action.Type)
        {
            case "addEv":
                return _addEvExecutor.ExecuteAsync(action, context, notificationSession);
            case "addEa":
                return _addEaExecutor.ExecuteAsync(action, context, notificationSession);
            case "addItem":
                return _addItemExecutor.ExecuteAsync(action, context, notificationSession);
            case "removeItem":
                return _removeItemExecutor.ExecuteAsync(action, context, notificationSession);
            case "addEffect":
                return _addEffectExecutor.ExecuteAsync(action, context, notificationSession);
            case "addCustomModifier":
                return _addCustomModifierExecutor.ExecuteAsync(action, context, notificationSession);
            default:
                return Task.CompletedTask;
        }
    }
}