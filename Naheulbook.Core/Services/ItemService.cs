using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Naheulbook.Core.Actions;
using Naheulbook.Core.Exceptions;
using Naheulbook.Core.Factories;
using Naheulbook.Core.Models;
using Naheulbook.Core.Notifications;
using Naheulbook.Core.Utils;
using Naheulbook.Data.Factories;
using Naheulbook.Data.Models;
using Naheulbook.Requests.Requests;
using Naheulbook.Shared.TransientModels;
using Naheulbook.Shared.Utils;

namespace Naheulbook.Core.Services
{
    public interface IItemService
    {
        Task<Item> AddItemToAsync(ItemOwnerType ownerType, int ownerId, CreateItemRequest request);
        Task<Item> AddRandomItemToAsync(ItemOwnerType ownerType, int ownerId, CreateRandomItemRequest request, int? currentUserId);
        Task<Item> UpdateItemDataAsync(NaheulbookExecutionContext executionContext, int itemId, ItemData itemData);
        Task<Item> UpdateItemModifiersAsync(NaheulbookExecutionContext executionContext, int itemId, IList<ActiveStatsModifier> itemModifiers);
        Task<Item> EquipItemAsync(NaheulbookExecutionContext executionContext, int itemId, EquipItemRequest request);
        Task<Item> ChangeItemContainerAsync(NaheulbookExecutionContext executionContext, int itemId, ChangeItemContainerRequest request);
        Task DeleteItemAsync(NaheulbookExecutionContext executionContext, int itemId);
        Task<(Item takenItem, int remainingQuantity)> TakeItemAsync(NaheulbookExecutionContext executionContext, int itemId, TakeItemRequest request);
        Task<int> GiveItemAsync(NaheulbookExecutionContext executionContext, int itemId, GiveItemRequest request);
        Task<IList<Item>> CreateItemsAsync(IList<CreateItemRequest>? requestItems);
        Task<Item> UseChargeAsync(NaheulbookExecutionContext executionContext, int itemId, UseChargeItemRequest request);
    }

    public class ItemService : IItemService
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly IItemFactory _itemFactory;
        private readonly INotificationSessionFactory _notificationSessionFactory;
        private readonly IAuthorizationUtil _authorizationUtil;
        private readonly IItemUtil _itemUtil;
        private readonly ICharacterHistoryUtil _characterHistoryUtil;
        private readonly IJsonUtil _jsonUtil;
        private readonly IRngUtil _rngUtil;
        private readonly IItemTemplateUtil _itemTemplateUtil;
        private readonly IActionsUtil _actionsUtil;
        private readonly IItemDataUtil _itemDataUtil;

        public ItemService(
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
        )
        {
            _unitOfWorkFactory = unitOfWorkFactory;
            _itemFactory = itemFactory;
            _notificationSessionFactory = notificationSessionFactory;
            _authorizationUtil = authorizationUtil;
            _itemUtil = itemUtil;
            _characterHistoryUtil = characterHistoryUtil;
            _jsonUtil = jsonUtil;
            _rngUtil = rngUtil;
            _itemTemplateUtil = itemTemplateUtil;
            _actionsUtil = actionsUtil;
            _itemDataUtil = itemDataUtil;
        }

        public async Task<Item> AddItemToAsync(
            ItemOwnerType ownerType,
            int ownerId,
            CreateItemRequest request
        )
        {
            using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
            {
                var itemTemplate = await uow.ItemTemplates.GetAsync(request.ItemTemplateId);
                if (itemTemplate == null)
                    throw new ItemTemplateNotFoundException(request.ItemTemplateId);

                var item = _itemFactory.CreateItem(ownerType, ownerId, itemTemplate, request.ItemData);

                uow.Items.Add(item);
                await uow.SaveChangesAsync();

                return (await uow.Items.GetWithAllDataAsync(item.Id))!;
            }
        }

        public async Task<Item> AddRandomItemToAsync(ItemOwnerType ownerType, int ownerId, CreateRandomItemRequest request, int? currentUserId)
        {
            using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
            {
                var itemTemplateSubCategory = await uow.ItemTemplateSubCategories.GetWithItemTemplatesByTechNameAsync(request.SubCategoryTechName);
                if (itemTemplateSubCategory == null)
                    throw new ItemTemplateSubCategoryNotFoundException(request.SubCategoryTechName);

                var itemTemplates = _itemTemplateUtil.FilterItemTemplatesBySource(itemTemplateSubCategory.ItemTemplates, currentUserId, false).ToList();
                if (itemTemplates.Count == 0)
                    throw new EmptyItemTemplateSubCategoryException(itemTemplateSubCategory.Id);

                var itemTemplateIndex = _rngUtil.GetRandomInt(0, itemTemplates.Count);
                var itemTemplate = itemTemplates.ElementAt(itemTemplateIndex);

                var item = _itemFactory.CreateItem(ownerType, ownerId, itemTemplate, new ItemData());

                uow.Items.Add(item);
                await uow.SaveChangesAsync();

                return (await uow.Items.GetWithAllDataAsync(item.Id))!;
            }
        }

        public async Task<Item> UpdateItemDataAsync(NaheulbookExecutionContext executionContext, int itemId, ItemData itemData)
        {
            using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
            {
                var item = await uow.Items.GetWithOwnerAsync(itemId);
                if (item == null)
                    throw new ItemNotFoundException(itemId);

                _authorizationUtil.EnsureItemAccess(executionContext, item);

                var currentItemData = _itemDataUtil.GetItemData(item);
                if (item.CharacterId.HasValue)
                {
                    if (itemData.Quantity != currentItemData.Quantity)
                        item.Character!.AddHistoryEntry(_characterHistoryUtil.CreateLogChangeItemQuantity(item.CharacterId.Value, item, currentItemData.Quantity, itemData.Quantity));
                    if (itemData.ReadCount.HasValue && itemData.ReadCount == currentItemData.ReadCount + 1)
                        item.Character!.AddHistoryEntry(_characterHistoryUtil.CreateLogReadBook(item.CharacterId.Value, item));
                    if (currentItemData.NotIdentified == true && itemData.NotIdentified == null)
                        item.Character!.AddHistoryEntry(_characterHistoryUtil.CreateLogIdentifyItem(item.CharacterId.Value, item));
                }

                _itemDataUtil.SetItemData(item, itemData);

                await uow.SaveChangesAsync();

                var session = _notificationSessionFactory.CreateSession();
                session.NotifyItemDataChanged(item);
                await session.CommitAsync();

                return item;
            }
        }

        public async Task<Item> UpdateItemModifiersAsync(NaheulbookExecutionContext executionContext, int itemId, IList<ActiveStatsModifier> itemModifiers)
        {
            using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
            {
                var item = await uow.Items.GetWithOwnerAsync(itemId);
                if (item == null)
                    throw new ItemNotFoundException(itemId);

                _authorizationUtil.EnsureItemAccess(executionContext, item);

                item.Modifiers = _jsonUtil.Serialize(itemModifiers);

                await uow.SaveChangesAsync();

                var session = _notificationSessionFactory.CreateSession();
                session.NotifyItemModifiersChanged(item);
                await session.CommitAsync();

                return item;
            }
        }

        public async Task<Item> EquipItemAsync(NaheulbookExecutionContext executionContext, int itemId, EquipItemRequest request)
        {
            using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
            {
                var item = await uow.Items.GetWithOwnerAsync(itemId);
                if (item == null)
                    throw new ItemNotFoundException(itemId);

                _authorizationUtil.EnsureItemAccess(executionContext, item);

                _itemUtil.EquipItem(item, request.Level);

                await uow.SaveChangesAsync();

                var session = _notificationSessionFactory.CreateSession();
                session.NotifyEquipItem(item);
                await session.CommitAsync();

                return item;
            }
        }

        public async Task<Item> ChangeItemContainerAsync(NaheulbookExecutionContext executionContext, int itemId, ChangeItemContainerRequest request)
        {
            using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
            {
                var item = await uow.Items.GetWithOwnerAsync(itemId);
                if (item == null)
                    throw new ItemNotFoundException(itemId);

                _authorizationUtil.EnsureItemAccess(executionContext, item);

                item.ContainerId = request.ContainerId;

                await uow.SaveChangesAsync();

                var session = _notificationSessionFactory.CreateSession();
                session.NotifyItemChangeContainer(item);
                await session.CommitAsync();

                return item;
            }
        }

        public async Task DeleteItemAsync(NaheulbookExecutionContext executionContext, int itemId)
        {
            using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
            {
                var item = await uow.Items.GetWithOwnerAsync(itemId);
                if (item == null)
                    throw new ItemNotFoundException(itemId);

                _authorizationUtil.EnsureItemAccess(executionContext, item);

                var session = _notificationSessionFactory.CreateSession();
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

                await uow.SaveChangesAsync();
                await session.CommitAsync();
            }
        }

        public async Task<(Item takenItem, int remainingQuantity)> TakeItemAsync(NaheulbookExecutionContext executionContext, int itemId, TakeItemRequest request)
        {
            using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
            {
                var item = await uow.Items.GetWithOwnerWitGroupCharactersAsync(itemId);
                if (item == null)
                    throw new ItemNotFoundException(itemId);

                _authorizationUtil.EnsureCanTakeItem(executionContext, item);

                var character = await uow.Characters.GetWithGroupAsync(request.CharacterId);
                if (character == null)
                    throw new CharacterNotFoundException(request.CharacterId);

                _authorizationUtil.EnsureCharacterAccess(executionContext, character);
            }


            return await _itemUtil.MoveItemToAsync(itemId, request.CharacterId, request.Quantity, MoveItemTrigger.TakeItemFromLoot);
        }

        public async Task<int> GiveItemAsync(NaheulbookExecutionContext executionContext, int itemId, GiveItemRequest request)
        {
            using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
            {
                var item = await uow.Items.GetWithOwnerWitGroupCharactersAsync(itemId);
                if (item == null)
                    throw new ItemNotFoundException(itemId);
                if (item.CharacterId == null)
                    throw new ForbiddenAccessException();

                _authorizationUtil.EnsureCharacterAccess(executionContext, item.Character!);

                if (item.Character!.Group!.Characters.All(x => x.Id != request.CharacterId && x.IsActive))
                    throw new CharacterNotFoundException(request.CharacterId);
            }


            var (_, remainingQuantity) = await _itemUtil.MoveItemToAsync(itemId, request.CharacterId, request.Quantity, MoveItemTrigger.GiveItem);

            return remainingQuantity;
        }

        public async Task<IList<Item>> CreateItemsAsync(IList<CreateItemRequest>? requestItems)
        {
            if (requestItems == null)
                return new List<Item>();
            if (requestItems.Count == 0)
                return new List<Item>();

            Dictionary<Guid, ItemTemplate> itemTemplatesById;
            using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
            {
                var itemTemplates = await uow.ItemTemplates.GetByIdsAsync(requestItems.Select(x => x.ItemTemplateId));
                itemTemplatesById = itemTemplates.ToDictionary(itemTemplate => itemTemplate.Id, itemTemplate => itemTemplate);
            }

            var items = new List<Item>();
            foreach (var requestItem in requestItems)
            {
                if (!itemTemplatesById.TryGetValue(requestItem.ItemTemplateId, out var itemTemplate))
                    throw new ItemTemplateNotFoundException(requestItem.ItemTemplateId);
                var item = _itemFactory.CreateItem(itemTemplate, requestItem.ItemData);
                items.Add(item);
            }

            return items;
        }

        public async Task<Item> UseChargeAsync(NaheulbookExecutionContext executionContext, int itemId, UseChargeItemRequest request)
        {
            using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
            {
                var usedItem = await uow.Items.GetWithAllDataWithCharacterAsync(itemId);
                if (usedItem == null)
                    throw new ItemNotFoundException(itemId);
                if (!usedItem.CharacterId.HasValue)
                    throw new InvalidItemOwnerTypeException(itemId);

                _authorizationUtil.EnsureItemAccess(executionContext, usedItem);

                var sourceCharacter = await uow.Characters.GetWithAllDataAsync(usedItem.CharacterId.Value);
                var targetCharacter = await uow.Characters.GetWithAllDataAsync(usedItem.CharacterId.Value);

                if (sourceCharacter == null)
                    throw new CharacterNotFoundException(usedItem.CharacterId.Value);
                if (targetCharacter == null)
                    throw new CharacterNotFoundException(usedItem.CharacterId.Value);

                if (sourceCharacter.GroupId != targetCharacter.GroupId)
                    throw new ForbiddenAccessException();

                var itemData = _itemDataUtil.GetItemData(usedItem);
                if (itemData.Charge < 1)
                    throw new NotChargeLeftOnItemException();

                _itemDataUtil.UpdateRelativeChargeCount(usedItem, -1);
                sourceCharacter.AddHistoryEntry(_characterHistoryUtil.CreateLogUseItemCharge(usedItem.CharacterId.Value, usedItem, itemData.Charge, itemData.Charge - 1));

                var notificationSession = _notificationSessionFactory.CreateSession();
                var context = new ActionContext(usedItem, sourceCharacter, targetCharacter, uow);
                var itemTemplateData = _itemTemplateUtil.GetItemTemplateData(usedItem.ItemTemplate);
                if (itemTemplateData.Actions == null)
                    throw new InvalidItemTemplateActionsDataException(usedItem.ItemTemplateId);
                foreach (var action in itemTemplateData.Actions)
                {
                    await _actionsUtil.ExecuteActionAsync(action, context, notificationSession);
                }

                await uow.SaveChangesAsync();
                await notificationSession.CommitAsync();

                return usedItem;
            }
        }
    }
}