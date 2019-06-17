using System;
using System.Threading.Tasks;
using Naheulbook.Core.Factories;
using Naheulbook.Core.Models;
using Naheulbook.Data.Factories;
using Naheulbook.Data.Models;
using Naheulbook.Requests.Requests;
using Newtonsoft.Json;

namespace Naheulbook.Core.Services
{
    public interface IItemService
    {
        Task<Item> AddItemToAsync(NaheulbookExecutionContext executionContext, ItemOwnerType ownerType, int ownerId, CreateItemRequest request);
    }

    public class ItemService : IItemService
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly IItemFactory _itemFactory;

        public ItemService(
            IUnitOfWorkFactory unitOfWorkFactory,
            IItemFactory itemFactory
        )
        {
            _unitOfWorkFactory = unitOfWorkFactory;
            _itemFactory = itemFactory;
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
    }
}