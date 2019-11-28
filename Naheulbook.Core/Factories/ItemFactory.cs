using System;
using Naheulbook.Core.Models;
using Naheulbook.Core.Utils;
using Naheulbook.Data.Models;
using Naheulbook.Shared.TransientModels;
using Naheulbook.Shared.Utils;

namespace Naheulbook.Core.Factories
{
    public interface IItemFactory
    {
        Item CreateItem(ItemOwnerType ownerType, int ownerId, ItemTemplate itemTemplate, ItemData itemData);
        Item CreateItem(ItemTemplate itemTemplate, ItemData itemData);
        Item CloneItem(Item originalItem);
    }

    public class ItemFactory : IItemFactory
    {
        private readonly IJsonUtil _jsonUtil;
        private readonly IItemDataUtil _itemDataUtil;

        public ItemFactory(IJsonUtil jsonUtil, IItemDataUtil itemDataUtil)
        {
            _jsonUtil = jsonUtil;
            _itemDataUtil = itemDataUtil;
        }

        public Item CreateItem(ItemOwnerType ownerType, int ownerId, ItemTemplate itemTemplate, ItemData itemData)
        {
            var item = CreateItem(itemTemplate, itemData);

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

        public Item CreateItem(ItemTemplate itemTemplate, ItemData itemData)
        {
            var itemTemplateData = _jsonUtil.DeserializeOrCreate<PartialItemTemplateData>(itemTemplate.Data);

            if (itemTemplateData.Charge.HasValue)
                itemData.Charge = itemTemplateData.Charge.Value;
            if (itemTemplateData.Lifetime != null)
                itemData.Lifetime = itemTemplateData.Lifetime;
            if (itemTemplateData.Quantifiable == true && itemData.Quantity == null)
                itemData.Quantity = 1;
            itemData.Icon ??= itemTemplateData.Icon;
            if (string.IsNullOrEmpty(itemData.Name))
            {
                if (itemData.NotIdentified == true && !string.IsNullOrEmpty(itemTemplateData.NotIdentifiedName))
                    itemData.Name = itemTemplateData.NotIdentifiedName;
                else
                    itemData.Name = itemTemplate.Name;
            }

            var item = new Item
            {
                Data = _jsonUtil.SerializeNonNull(itemData),
                ItemTemplateId = itemTemplate.Id
            };
            return item;
        }

        public Item CloneItem(Item originalItem)
        {
            var originItemData = _itemDataUtil.GetItemData(originalItem);
            var clonedItem = new Item
            {
                Modifiers = originalItem.Modifiers,
                ItemTemplateId = originalItem.ItemTemplateId,
                Data = _jsonUtil.SerializeNonNull(originItemData)
            };

            return clonedItem;
        }
    }
}