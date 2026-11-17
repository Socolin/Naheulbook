using Naheulbook.Core.Features.Character;
using Naheulbook.Core.Features.Item.Actions;
using Naheulbook.Core.Features.Shared;
using Naheulbook.Core.Notifications;
using Naheulbook.Data.EntityFrameworkCore.Entities;
using Naheulbook.Data.UnitOfWorks;
using Naheulbook.Requests.Requests;
using Naheulbook.Shared.TransientModels;
using Naheulbook.Shared.Utils;

namespace Naheulbook.Core.Features.Item;

public interface IItemService
{
    Task<ItemEntity> AddItemToAsync(ItemOwnerType ownerType, int ownerId, CreateItemRequest request, CancellationToken cancellationToken = default);
    Task<ItemEntity> AddRandomItemToAsync(ItemOwnerType ownerType, int ownerId, CreateRandomItemRequest request, int? currentUserId, CancellationToken cancellationToken = default);
    Task<ItemEntity> UpdateItemDataAsync(NaheulbookExecutionContext executionContext, int itemId, ItemData itemData, CancellationToken cancellationToken = default);
    Task<ItemEntity> UpdateItemModifiersAsync(NaheulbookExecutionContext executionContext, int itemId, IList<ActiveStatsModifier> itemModifiers, CancellationToken cancellationToken = default);
    Task<ItemEntity> EquipItemAsync(NaheulbookExecutionContext executionContext, int itemId, EquipItemRequest request, CancellationToken cancellationToken = default);
    Task<ItemEntity> ChangeItemContainerAsync(NaheulbookExecutionContext executionContext, int itemId, ChangeItemContainerRequest request, CancellationToken cancellationToken = default);
    Task DeleteItemAsync(NaheulbookExecutionContext executionContext, int itemId, CancellationToken cancellationToken = default);
    Task<(ItemEntity takenItem, int remainingQuantity)> TakeItemAsync(NaheulbookExecutionContext executionContext, int itemId, TakeItemRequest request, CancellationToken cancellationToken = default);
    Task<int> GiveItemAsync(NaheulbookExecutionContext executionContext, int itemId, GiveItemRequest request, CancellationToken cancellationToken = default);
    Task<IList<ItemEntity>> CreateItemsAsync(IList<CreateItemRequest>? requestItems, CancellationToken cancellationToken = default);
    Task<ItemEntity> UseChargeAsync(NaheulbookExecutionContext executionContext, int itemId, UseChargeItemRequest request, CancellationToken cancellationToken = default);
}

public class ItemService(
    IUnitOfWorkFactory unitOfWorkFactory,
    IItemFactory itemFactory,
    INotificationSessionFactory notificationSessionFactory,
    IAuthorizationUtil authorizationUtil,
    IItemUtil itemUtil,
    ICharacterHistoryUtil characterHistoryUtil,
    IJsonUtil jsonUtil,
    IRngUtil rngUtil,
    IItemTemplateUtil itemTemplateUtil,
    IActionsUtil actionsUtil,
    IItemDataUtil itemDataUtil
) : IItemService
{
    public async Task<ItemEntity> AddItemToAsync(
        ItemOwnerType ownerType,
        int ownerId,
        CreateItemRequest request,
        CancellationToken cancellationToken = default
    )
    {
        using var uow = unitOfWorkFactory.CreateUnitOfWork();

        var itemTemplate = await uow.ItemTemplates.GetAsync(request.ItemTemplateId, cancellationToken);
        if (itemTemplate == null)
            throw new ItemTemplateNotFoundException(request.ItemTemplateId);

        var item = itemFactory.CreateItem(ownerType, ownerId, itemTemplate, request.ItemData);

        uow.Items.Add(item);
        await uow.SaveChangesAsync(DoNotInterrupt.Token);

        return (await uow.Items.GetWithAllDataAsync(item.Id, cancellationToken))!;
    }

    public async Task<ItemEntity> AddRandomItemToAsync(ItemOwnerType ownerType, int ownerId, CreateRandomItemRequest request, int? currentUserId, CancellationToken cancellationToken = default)
    {
        using var uow = unitOfWorkFactory.CreateUnitOfWork();
        var itemTemplateSubCategory = await uow.ItemTemplateSubCategories.GetWithItemTemplatesByTechNameAsync(request.SubCategoryTechName, cancellationToken);
        if (itemTemplateSubCategory == null)
            throw new ItemTemplateSubCategoryNotFoundException(request.SubCategoryTechName);

        var itemTemplates = itemTemplateUtil.FilterItemTemplatesBySource(itemTemplateSubCategory.ItemTemplates, currentUserId, false).ToList();
        if (itemTemplates.Count == 0)
            throw new EmptyItemTemplateSubCategoryException(itemTemplateSubCategory.Id);

        var itemTemplateIndex = rngUtil.GetRandomInt(0, itemTemplates.Count);
        var itemTemplate = itemTemplates.ElementAt(itemTemplateIndex);

        var item = itemFactory.CreateItem(ownerType, ownerId, itemTemplate, new ItemData());

        uow.Items.Add(item);
        await uow.SaveChangesAsync(DoNotInterrupt.Token);

        return (await uow.Items.GetWithAllDataAsync(item.Id, cancellationToken))!;
    }

    public async Task<ItemEntity> UpdateItemDataAsync(NaheulbookExecutionContext executionContext, int itemId, ItemData itemData, CancellationToken cancellationToken = default)
    {
        using var uow = unitOfWorkFactory.CreateUnitOfWork();
        var item = await uow.Items.GetWithOwnerAsync(itemId, cancellationToken);
        if (item == null)
            throw new ItemNotFoundException(itemId);

        authorizationUtil.EnsureItemAccess(executionContext, item);

        var currentItemData = itemDataUtil.GetItemData(item);
        if (item.CharacterId.HasValue)
        {
            if (itemData.Quantity != currentItemData.Quantity)
                item.Character!.AddHistoryEntry(characterHistoryUtil.CreateLogChangeItemQuantity(item.CharacterId.Value, item, currentItemData.Quantity, itemData.Quantity));
            if (itemData.ReadCount.HasValue && itemData.ReadCount == currentItemData.ReadCount + 1)
                item.Character!.AddHistoryEntry(characterHistoryUtil.CreateLogReadBook(item.CharacterId.Value, item));
            if (currentItemData.NotIdentified == true && itemData.NotIdentified == null)
                item.Character!.AddHistoryEntry(characterHistoryUtil.CreateLogIdentifyItem(item.CharacterId.Value, item));
        }

        itemDataUtil.SetItemData(item, itemData);

        await uow.SaveChangesAsync(DoNotInterrupt.Token);

        var session = notificationSessionFactory.CreateSession();
        session.NotifyItemDataChanged(item);
        await session.CommitAsync();

        return item;
    }

    public async Task<ItemEntity> UpdateItemModifiersAsync(NaheulbookExecutionContext executionContext, int itemId, IList<ActiveStatsModifier> itemModifiers, CancellationToken cancellationToken = default)
    {
        using var uow = unitOfWorkFactory.CreateUnitOfWork();
        var item = await uow.Items.GetWithOwnerAsync(itemId, cancellationToken);
        if (item == null)
            throw new ItemNotFoundException(itemId);

        authorizationUtil.EnsureItemAccess(executionContext, item);

        item.Modifiers = jsonUtil.Serialize(itemModifiers);

        await uow.SaveChangesAsync(DoNotInterrupt.Token);

        var session = notificationSessionFactory.CreateSession();
        session.NotifyItemModifiersChanged(item);
        await session.CommitAsync();

        return item;
    }

    public async Task<ItemEntity> EquipItemAsync(NaheulbookExecutionContext executionContext, int itemId, EquipItemRequest request, CancellationToken cancellationToken = default)
    {
        using var uow = unitOfWorkFactory.CreateUnitOfWork();
        var item = await uow.Items.GetWithOwnerAsync(itemId, cancellationToken);
        if (item == null)
            throw new ItemNotFoundException(itemId);

        authorizationUtil.EnsureItemAccess(executionContext, item);

        itemUtil.EquipItem(item, request.Level);

        await uow.SaveChangesAsync(DoNotInterrupt.Token);

        var session = notificationSessionFactory.CreateSession();
        session.NotifyEquipItem(item);
        await session.CommitAsync();

        return item;
    }

    public async Task<ItemEntity> ChangeItemContainerAsync(NaheulbookExecutionContext executionContext, int itemId, ChangeItemContainerRequest request, CancellationToken cancellationToken = default)
    {
        using var uow = unitOfWorkFactory.CreateUnitOfWork();
        var item = await uow.Items.GetWithOwnerAsync(itemId, cancellationToken);
        if (item == null)
            throw new ItemNotFoundException(itemId);

        authorizationUtil.EnsureItemAccess(executionContext, item);

        item.ContainerId = request.ContainerId;

        await uow.SaveChangesAsync(DoNotInterrupt.Token);

        var session = notificationSessionFactory.CreateSession();
        session.NotifyItemChangeContainer(item);
        await session.CommitAsync();

        return item;
    }

    public async Task DeleteItemAsync(NaheulbookExecutionContext executionContext, int itemId, CancellationToken cancellationToken = default)
    {
        using var uow = unitOfWorkFactory.CreateUnitOfWork();
        var item = await uow.Items.GetWithOwnerAsync(itemId, cancellationToken);
        if (item == null)
            throw new ItemNotFoundException(itemId);

        authorizationUtil.EnsureItemAccess(executionContext, item);

        var session = notificationSessionFactory.CreateSession();
        session.NotifyItemDeleteItem(item);

        // TODO: Rework history to be able to delete items in all case
        if (item.CharacterId.HasValue)
        {
            item.CharacterId = null;
            item.ContainerId = null;
        }
        else
        {
            uow.Items.Remove(item);
        }

        await uow.SaveChangesAsync(DoNotInterrupt.Token);
        await session.CommitAsync();
    }

    public async Task<(ItemEntity takenItem, int remainingQuantity)> TakeItemAsync(NaheulbookExecutionContext executionContext, int itemId, TakeItemRequest request, CancellationToken cancellationToken = default)
    {
        using (var uow = unitOfWorkFactory.CreateUnitOfWork())
        {
            var item = await uow.Items.GetWithOwnerWitGroupCharactersAsync(itemId, cancellationToken);
            if (item == null)
                throw new ItemNotFoundException(itemId);

            authorizationUtil.EnsureCanTakeItem(executionContext, item);

            var character = await uow.Characters.GetWithGroupAsync(request.CharacterId, cancellationToken);
            if (character == null)
                throw new CharacterNotFoundException(request.CharacterId);

            authorizationUtil.EnsureCharacterAccess(executionContext, character);
        }


        return await itemUtil.MoveItemToAsync(itemId, request.CharacterId, request.Quantity, MoveItemTrigger.TakeItemFromLoot, cancellationToken);
    }

    public async Task<int> GiveItemAsync(NaheulbookExecutionContext executionContext, int itemId, GiveItemRequest request, CancellationToken cancellationToken = default)
    {
        using (var uow = unitOfWorkFactory.CreateUnitOfWork())
        {
            var item = await uow.Items.GetWithOwnerWitGroupCharactersAsync(itemId, cancellationToken);
            if (item == null)
                throw new ItemNotFoundException(itemId);
            if (item.CharacterId == null)
                throw new ForbiddenAccessException();

            authorizationUtil.EnsureCharacterAccess(executionContext, item.Character!);

            if (item.Character!.Group!.Characters.All(x => x.Id != request.CharacterId && x.IsActive))
                throw new CharacterNotFoundException(request.CharacterId);
        }


        var (_, remainingQuantity) = await itemUtil.MoveItemToAsync(itemId, request.CharacterId, request.Quantity, MoveItemTrigger.GiveItem, cancellationToken);

        return remainingQuantity;
    }

    public async Task<IList<ItemEntity>> CreateItemsAsync(IList<CreateItemRequest>? requestItems, CancellationToken cancellationToken = default)
    {
        if (requestItems == null)
            return new List<ItemEntity>();
        if (requestItems.Count == 0)
            return new List<ItemEntity>();

        Dictionary<Guid, ItemTemplateEntity> itemTemplatesById;
        using (var uow = unitOfWorkFactory.CreateUnitOfWork())
        {
            var itemTemplates = await uow.ItemTemplates.GetByIdsAsync(requestItems.Select(x => x.ItemTemplateId), cancellationToken);
            itemTemplatesById = itemTemplates.ToDictionary(itemTemplate => itemTemplate.Id, itemTemplate => itemTemplate);
        }

        var items = new List<ItemEntity>();
        foreach (var requestItem in requestItems)
        {
            if (!itemTemplatesById.TryGetValue(requestItem.ItemTemplateId, out var itemTemplate))
                throw new ItemTemplateNotFoundException(requestItem.ItemTemplateId);
            var item = itemFactory.CreateItem(itemTemplate, requestItem.ItemData);
            items.Add(item);
        }

        return items;
    }

    public async Task<ItemEntity> UseChargeAsync(NaheulbookExecutionContext executionContext, int itemId, UseChargeItemRequest request, CancellationToken cancellationToken = default)
    {
        using var uow = unitOfWorkFactory.CreateUnitOfWork();
        var usedItem = await uow.Items.GetWithAllDataWithCharacterAsync(itemId, cancellationToken);
        if (usedItem == null)
            throw new ItemNotFoundException(itemId);
        if (!usedItem.CharacterId.HasValue)
            throw new InvalidItemOwnerTypeException(itemId);

        authorizationUtil.EnsureItemAccess(executionContext, usedItem);

        var sourceCharacter = await uow.Characters.GetWithAllDataAsync(usedItem.CharacterId.Value, cancellationToken);
        var targetCharacter = await uow.Characters.GetWithAllDataAsync(usedItem.CharacterId.Value, cancellationToken);

        if (sourceCharacter == null)
            throw new CharacterNotFoundException(usedItem.CharacterId.Value);
        if (targetCharacter == null)
            throw new CharacterNotFoundException(usedItem.CharacterId.Value);

        if (sourceCharacter.GroupId != targetCharacter.GroupId)
            throw new ForbiddenAccessException();

        var itemData = itemDataUtil.GetItemData(usedItem);
        if (itemData.Charge < 1)
            throw new NotChargeLeftOnItemException();

        itemDataUtil.UpdateRelativeChargeCount(usedItem, -1);
        sourceCharacter.AddHistoryEntry(characterHistoryUtil.CreateLogUseItemCharge(usedItem.CharacterId.Value, usedItem, itemData.Charge, itemData.Charge - 1));

        var notificationSession = notificationSessionFactory.CreateSession();
        var context = new ActionContext(usedItem, sourceCharacter, targetCharacter, uow);
        var itemTemplateData = itemTemplateUtil.GetItemTemplateData(usedItem.ItemTemplate);
        if (itemTemplateData.Actions == null)
            throw new InvalidItemTemplateActionsDataException(usedItem.ItemTemplateId);
        foreach (var action in itemTemplateData.Actions)
        {
            await actionsUtil.ExecuteActionAsync(action, context, notificationSession, cancellationToken);
        }

        await uow.SaveChangesAsync(DoNotInterrupt.Token);
        await notificationSession.CommitAsync();

        return usedItem;
    }
}