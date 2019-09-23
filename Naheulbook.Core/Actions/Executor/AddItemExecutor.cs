using System.Threading.Tasks;
using Naheulbook.Core.Exceptions;
using Naheulbook.Core.Factories;
using Naheulbook.Core.Models;
using Naheulbook.Core.Notifications;
using Naheulbook.Shared.TransientModels;

namespace Naheulbook.Core.Actions.Executor
{
    public interface IAddItemExecutor : IActionExecutor
    {
    }

    public class AddItemExecutor : IAddItemExecutor
    {
        private const string ActionType = "addItem";
        private readonly IItemFactory _itemFactory;

        public AddItemExecutor(
            IItemFactory itemFactory
        )
        {
            _itemFactory = itemFactory;
        }

        public async Task ExecuteAsync(
            NhbkAction action,
            ActionContext context,
            INotificationSession notificationSession
        )
        {
            if (action.Type != ActionType)
                throw new InvalidActionTypeException(action.Type, ActionType);
            if (!action.Data.TemplateId.HasValue)
                throw new InvalidActionDataException(action.Type);

            var itemTemplate = await context.UnitOfWork.ItemTemplates.GetAsync(action.Data.TemplateId.Value);
            if (itemTemplate == null)
                throw new ItemTemplateNotFoundException(action.Data.TemplateId.Value);

            var itemData = new ItemData();
            if (!string.IsNullOrEmpty(action.Data.ItemName))
                itemData.Name = action.Data.ItemName;
            if (action.Data.Quantity.HasValue)
                itemData.Quantity = action.Data.Quantity.Value;

            var item = _itemFactory.CreateItem(ItemOwnerType.Character, context.SourceCharacter.Id, itemTemplate, itemData);
            context.UnitOfWork.Items.Add(item);

            notificationSession.NotifyCharacterAddItem(context.SourceCharacter.Id, item);
        }
    }
}