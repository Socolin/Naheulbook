using Naheulbook.Core.Notifications;
using Naheulbook.Shared.TransientModels;

namespace Naheulbook.Core.Features.Item.Actions.Executor;

public interface IRemoveItemExecutor : IActionExecutor;

public class RemoveItemExecutor(IItemUtil itemUtil) : IRemoveItemExecutor
{
    private const string ActionType = "removeItem";

    public Task ExecuteAsync(
        NhbkAction action,
        ActionContext context,
        INotificationSession notificationSession
    )
    {
        if (action.Type != ActionType)
            throw new InvalidActionTypeException(action.Type, ActionType);

        if (itemUtil.DecrementQuantityOrDeleteItem(context.UsedItem))
        {
            context.UnitOfWork.Items.Remove(context.UsedItem);
            notificationSession.NotifyItemDeleteItem(context.UsedItem);
        }

        return Task.CompletedTask;
    }
}