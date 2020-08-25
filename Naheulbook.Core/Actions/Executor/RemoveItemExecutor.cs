using System.Threading.Tasks;
using Naheulbook.Core.Exceptions;
using Naheulbook.Core.Notifications;
using Naheulbook.Core.Utils;
using Naheulbook.Shared.TransientModels;

namespace Naheulbook.Core.Actions.Executor
{
    public interface IRemoveItemExecutor : IActionExecutor
    {
    }

    public class RemoveItemExecutor : IRemoveItemExecutor
    {
        private readonly IItemUtil _itemUtil;
        private const string ActionType = "removeItem";

        public RemoveItemExecutor(
            IItemUtil itemUtil
        )
        {
            _itemUtil = itemUtil;
        }

        public Task ExecuteAsync(
            NhbkAction action,
            ActionContext context,
            INotificationSession notificationSession
        )
        {
            if (action.Type != ActionType)
                throw new InvalidActionTypeException(action.Type, ActionType);

            if (_itemUtil.DecrementQuantityOrDeleteItem(context.UsedItem))
            {
                context.UnitOfWork.Items.Remove(context.UsedItem);
                notificationSession.NotifyItemDeleteItem(context.UsedItem);
            }

            return Task.CompletedTask;
        }
    }
}