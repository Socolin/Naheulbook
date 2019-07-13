using System;
using Naheulbook.Core.Models;
using Naheulbook.Data.Models;
using Naheulbook.Shared.TransientModels;
using Naheulbook.Shared.Utils;

namespace Naheulbook.Core.Factories
{
    public interface IItemFactory
    {
        Item CreateItemFromRequest(ItemOwnerType ownerType, int ownerId, ItemTemplate itemTemplate, ItemData itemData);
    }

    public class ItemFactory : IItemFactory
    {
        private readonly IJsonUtil _jsonUtil;

        public ItemFactory(IJsonUtil jsonUtil)
        {
            _jsonUtil = jsonUtil;
        }

        public Item CreateItemFromRequest(ItemOwnerType ownerType, int ownerId, ItemTemplate itemTemplate, ItemData itemData)
        {
            var itemTemplateData = _jsonUtil.Deserialize<PartialItemTemplateData>(itemTemplate.Data) ?? new PartialItemTemplateData();

            if (itemTemplateData.Charge.HasValue)
                itemData.Charge = itemTemplateData.Charge.Value;
            if (itemTemplateData.Icon != null)
                itemData.Icon = itemTemplateData.Icon;
            if (itemTemplateData.Lifetime != null)
                itemData.Lifetime = itemTemplateData.Lifetime;

            var item = new Item
            {
                Data = _jsonUtil.Serialize(itemData),
                ItemTemplate = itemTemplate
            };

            switch (ownerType)
            {
                case ItemOwnerType.Character:
                    item.CharacterId = ownerId;
                    break;
                case ItemOwnerType.Loot:
                    item.LootId = ownerId;
                    break;
                case ItemOwnerType.Monster:
                    item.MonsterId = ownerId;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(ownerType), ownerType, null);
            }

            return item;
        }
    }
}