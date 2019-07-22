using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        Task<int> GiveItemAsync(NaheulbookExecutionContext executionContext, int itemId, TakeItemRequest request);
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

        public ItemService(
            IUnitOfWorkFactory unitOfWorkFactory,
            IItemFactory itemFactory,
            INotificationSessionFactory notificationSessionFactory,
            IAuthorizationUtil authorizationUtil,
            IItemUtil itemUtil,
            ICharacterHistoryUtil characterHistoryUtil,
            IJsonUtil jsonUtil,
            IRngUtil rngUtil,
            IItemTemplateUtil itemTemplateUtil
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
                await uow.CompleteAsync();

                return await uow.Items.GetWithAllDataAsync(item.Id);
            }
        }

        public async Task<Item> AddRandomItemToAsync(ItemOwnerType ownerType, int ownerId, CreateRandomItemRequest request, int? currentUserId)
        {
            using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
            {
                var itemTemplateCategory = await uow.ItemTemplateCategories.GetWithItemTemplatesByTechNameAsync(request.CategoryTechName);
                if (itemTemplateCategory == null)
                    throw new ItemTemplateCategoryNotFoundException(request.CategoryTechName);

                var itemTemplates = _itemTemplateUtil.FilterItemTemplatesBySource(itemTemplateCategory.ItemTemplates, currentUserId, false).ToList();
                if (itemTemplates.Count == 0)
                    throw new EmptyItemTemplateCategoryException(itemTemplateCategory.Id);

                var itemTemplateIndex = _rngUtil.GetRandomInt(0, itemTemplates.Count);
                var itemTemplate = itemTemplates.ElementAt(itemTemplateIndex);

                var item = _itemFactory.CreateItem(ownerType, ownerId, itemTemplate, new ItemData());

                uow.Items.Add(item);
                await uow.CompleteAsync();

                return await uow.Items.GetWithAllDataAsync(item.Id);
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

                var currentItemData = _jsonUtil.Deserialize<ItemData>(item.Data) ?? new ItemData();
                if (item.CharacterId.HasValue)
                {
                    if (itemData.Quantity != currentItemData.Quantity)
                        item.Character.AddHistoryEntry(_characterHistoryUtil.CreateLogChangeItemQuantity(item.CharacterId.Value, item, currentItemData.Quantity, itemData.Quantity));
                    if (itemData.Charge.HasValue && itemData.Charge == currentItemData.Charge - 1) // TODO: execute item actions when a charge is used
                        item.Character.AddHistoryEntry(_characterHistoryUtil.CreateLogUseItemCharge(item.CharacterId.Value, item, currentItemData.Charge, itemData.Charge));
                    if (itemData.ReadCount.HasValue && itemData.ReadCount == currentItemData.ReadCount + 1)
                        item.Character.AddHistoryEntry(_characterHistoryUtil.CreateLogReadBook(item.CharacterId.Value, item));
                    if (currentItemData.NotIdentified == true && itemData.NotIdentified == null)
                        item.Character.AddHistoryEntry(_characterHistoryUtil.CreateLogIdentifyItem(item.CharacterId.Value, item));
                }

                item.Data = _jsonUtil.Serialize(itemData);

                await uow.CompleteAsync();

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

                await uow.CompleteAsync();

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

                await uow.CompleteAsync();

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

                await uow.CompleteAsync();

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

                await uow.CompleteAsync();
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

        public async Task<int> GiveItemAsync(NaheulbookExecutionContext executionContext, int itemId, TakeItemRequest request)
        {
            using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
            {
                var item = await uow.Items.GetWithOwnerWitGroupCharactersAsync(itemId);
                if (item == null)
                    throw new ItemNotFoundException(itemId);
                if (item.CharacterId == null)
                    throw new ForbiddenAccessException();

                _authorizationUtil.EnsureCharacterAccess(executionContext, item.Character);

                if (item.Character.Group.Characters.All(x => x.Id != request.CharacterId && x.IsActive))
                    throw new CharacterNotFoundException(request.CharacterId);
            }


            var (_, remainingQuantity) = await _itemUtil.MoveItemToAsync(itemId, request.CharacterId, request.Quantity, MoveItemTrigger.GiveItem);

            return remainingQuantity;
        }
    }
}