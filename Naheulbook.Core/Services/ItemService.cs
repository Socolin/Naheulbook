using System.Collections.Generic;
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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Naheulbook.Core.Services
{
    public interface IItemService
    {
        Task<Item> AddItemToAsync(NaheulbookExecutionContext executionContext, ItemOwnerType ownerType, int ownerId, CreateItemRequest request);
        Task<Item> UpdateItemDataAsync(NaheulbookExecutionContext executionContext, int itemId, JObject itemData);
        Task<Item> UpdateItemModifiersAsync(NaheulbookExecutionContext executionContext, int itemId, IList<ActiveStatsModifier> itemModifiers);
        Task<Item> EquipItemAsync(NaheulbookExecutionContext executionContext, int itemId, EquipItemRequest request);
        Task<Item> ChangeItemContainerAsync(NaheulbookExecutionContext executionContext, int itemId, ChangeItemContainerRequest request);
        Task DeleteItemAsync(NaheulbookExecutionContext executionContext, int itemId);
    }

    public class ItemService : IItemService
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly IItemFactory _itemFactory;
        private readonly INotificationSessionFactory _notificationSessionFactory;
        private readonly IAuthorizationUtil _authorizationUtil;
        private readonly IItemUtil _itemUtil;
        private readonly IJsonUtil _jsonUtil;

        public ItemService(
            IUnitOfWorkFactory unitOfWorkFactory,
            IItemFactory itemFactory,
            INotificationSessionFactory notificationSessionFactory,
            IAuthorizationUtil authorizationUtil,
            IItemUtil itemUtil,
            IJsonUtil jsonUtil
        )
        {
            _unitOfWorkFactory = unitOfWorkFactory;
            _itemFactory = itemFactory;
            _notificationSessionFactory = notificationSessionFactory;
            _authorizationUtil = authorizationUtil;
            _itemUtil = itemUtil;
            _jsonUtil = jsonUtil;
        }

        public async Task<Item> AddItemToAsync(
            NaheulbookExecutionContext executionContext,
            ItemOwnerType ownerType,
            int ownerId,
            CreateItemRequest request
        )
        {
            using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
            {
                var item = _itemFactory.CreateItemFromRequest(ownerType, ownerId, request);

                uow.Items.Add(item);
                await uow.CompleteAsync();

                return await uow.Items.GetWithAllDataAsync(item.Id);
            }
        }

        public async Task<Item> UpdateItemDataAsync(NaheulbookExecutionContext executionContext, int itemId, JObject itemData)
        {
            using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
            {
                var item = await uow.Items.GetWithOwnerAsync(itemId);
                if (item == null)
                    throw new ItemNotFoundException(itemId);

                _authorizationUtil.EnsureItemAccess(executionContext, item);

                item.Data = itemData.ToString(Formatting.None);

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
    }
}