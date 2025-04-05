using System;
using Naheulbook.Data.EntityFrameworkCore.Entities;
using Naheulbook.Shared.TransientModels;
using Naheulbook.Shared.Utils;

namespace Naheulbook.Core.Features.Item;

public interface IItemFactory
{
    ItemEntity CreateItem(ItemOwnerType ownerType, int ownerId, ItemTemplateEntity itemTemplate, ItemData itemData);
    ItemEntity CreateItem(ItemTemplateEntity itemTemplate, ItemData itemData);
    ItemEntity CloneItem(ItemEntity originalItem);
}

public class ItemFactory(IJsonUtil jsonUtil, IItemDataUtil itemDataUtil) : IItemFactory
{
    public ItemEntity CreateItem(ItemOwnerType ownerType, int ownerId, ItemTemplateEntity itemTemplate, ItemData itemData)
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

    public ItemEntity CreateItem(ItemTemplateEntity itemTemplate, ItemData itemData)
    {
        var itemTemplateData = jsonUtil.DeserializeOrCreate<PartialItemTemplateData>(itemTemplate.Data);

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

        var item = new ItemEntity
        {
            Data = jsonUtil.SerializeNonNull(itemData),
            ItemTemplateId = itemTemplate.Id,
        };
        return item;
    }

    public ItemEntity CloneItem(ItemEntity originalItem)
    {
        var originItemData = itemDataUtil.GetItemData(originalItem);
        var clonedItem = new ItemEntity
        {
            Modifiers = originalItem.Modifiers,
            ItemTemplateId = originalItem.ItemTemplateId,
            Data = jsonUtil.SerializeNonNull(originItemData),
        };

        return clonedItem;
    }
}