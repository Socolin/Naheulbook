﻿using Naheulbook.Core.Notifications;
using Naheulbook.Shared.TransientModels;

namespace Naheulbook.Core.Features.Item.Actions.Executor;

public interface IAddItemExecutor : IActionExecutor;

public class AddItemExecutor(IItemFactory itemFactory) : IAddItemExecutor
{
    private const string ActionType = "addItem";

    public async Task ExecuteAsync(
        NhbkAction action,
        ActionContext context,
        INotificationSession notificationSession
    )
    {
        if (action.Type != ActionType)
            throw new InvalidActionTypeException(action.Type, ActionType);
        if (action.Data == null)
            throw new InvalidActionDataException(action.Type);
        if (!action.Data.TemplateId.HasValue)
            throw new InvalidActionDataException(action.Type);

        var itemTemplate = await context.UnitOfWork.ItemTemplates.GetWithModifiersWithRequirementsWithSkillsWithSkillModifiersWithSlotsWithUnSkillsAsync(action.Data.TemplateId.Value);
        if (itemTemplate == null)
            throw new ItemTemplateNotFoundException(action.Data.TemplateId.Value);

        var itemData = new ItemData();
        if (!string.IsNullOrEmpty(action.Data.ItemName))
            itemData.Name = action.Data.ItemName;
        if (action.Data.Quantity.HasValue)
            itemData.Quantity = action.Data.Quantity.Value;

        var item = itemFactory.CreateItem(ItemOwnerType.Character, context.SourceCharacter.Id, itemTemplate, itemData);
        context.UnitOfWork.Items.Add(item);

        notificationSession.NotifyCharacterAddItem(context.SourceCharacter.Id, item, true);
    }
}