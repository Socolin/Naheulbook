using System.Collections.Generic;
using System.Threading.Tasks;
using Naheulbook.Core.Exceptions;
using Naheulbook.Core.Factories;
using Naheulbook.Core.Models;
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
    }

    public class ItemService : IItemService
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly IItemFactory _itemFactory;
        private readonly IChangeNotifier _changeNotifier;
        private readonly IAuthorizationUtil _authorizationUtil;
        private readonly IJsonUtil _jsonUtil;

        public ItemService(
            IUnitOfWorkFactory unitOfWorkFactory,
            IItemFactory itemFactory,
            IChangeNotifier changeNotifier,
            IAuthorizationUtil authorizationUtil,
            IJsonUtil jsonUtil
        )
        {
            _unitOfWorkFactory = unitOfWorkFactory;
            _itemFactory = itemFactory;
            _changeNotifier = changeNotifier;
            _authorizationUtil = authorizationUtil;
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
                // TODO: Check permissions
                // TODO: Character / Group history ?

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

                await _changeNotifier.NotifyItemDataChangedAsync(item);

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

                await _changeNotifier.NotifyItemModifiersChangedAsync(item);

                return item;
            }
        }
    }
}