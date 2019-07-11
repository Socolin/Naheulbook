using System;
using Naheulbook.Core.Models;
using Naheulbook.Core.Utils;
using Naheulbook.Data.Models;
using Naheulbook.Requests.Requests;
using Naheulbook.Shared.Utils;

namespace Naheulbook.Core.Factories
{
    public interface IItemFactory
    {
        Item CreateItemFromRequest(ItemOwnerType ownerType, int ownerId, CreateItemRequest request);
    }

    public class ItemFactory : IItemFactory
    {
        private readonly IJsonUtil _jsonUtil;

        public ItemFactory(IJsonUtil jsonUtil)
        {
            _jsonUtil = jsonUtil;
        }

        public Item CreateItemFromRequest(ItemOwnerType ownerType, int ownerId, CreateItemRequest request)
        {
            var item = new Item
            {
                Data = _jsonUtil.Serialize(request.ItemData),
                ItemTemplateId = request.ItemTemplateId
            };

            switch (ownerType)
            {
                case ItemOwnerType.Character:
                    item.CharacterId = ownerId;
                    break;
                case ItemOwnerType.Loot:
                    item.LootId = ownerId;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(ownerType), ownerType, null);
            }

            return item;
        }
    }
}