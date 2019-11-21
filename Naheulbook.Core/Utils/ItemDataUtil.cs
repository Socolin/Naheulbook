using System;
using Naheulbook.Data.Models;
using Naheulbook.Shared.TransientModels;
using Naheulbook.Shared.Utils;

namespace Naheulbook.Core.Utils
{
    public interface IItemDataUtil
    {
        IReadOnlyItemData GetItemData(Item item);
        void SetItemData(Item item, IReadOnlyItemData itemData);

        bool IsItemEquipped(Item item);
        void UpdateEquipItem(Item item, int? level);
        void UpdateQuantity(Item item, int newQuantity);
        void UpdateRelativeQuantity(Item item, int quantityChange);
        void UpdateChargeCount(Item item, int newChargeCount);
        void UpdateRelativeChargeCount(Item item, int change);
        void ResetReadCount(Item item);
    }

    /// <summary>
    /// Note about this class. The methods are think to be the safest possible and limit risk of missuses of `ItemData`
    /// for example avoid mixing item data between two items etc..
    /// So this ends up with something with terrible performance, but that's ok until some measures prove that
    /// parsing/serializing of item data json is a problem. Then a clever method and more complex would be to add
    /// an ItemDataCache on Item, that would be fill by deserializing from Item.Data when accessed, then
    /// when any modification occurs on that object, a variable `changed` is set to true, and when someone try to
    /// access Data using {get} it check if ItemDataCache?.changed == true and then serialized if needed, or do nothing
    /// This will limit parsing / serializing of json to only one occurence. (Careful to not use that cache object in
    /// returns of GetItemData, GetItemData return an immutable object, but an optiimization would be to return a copy)
    /// of that cache object (one new + copy 10/12 fields should be fast enough)
    /// </summary>
    public class ItemDataUtil : IItemDataUtil
    {
        private readonly IJsonUtil _jsonUtil;

        public ItemDataUtil(IJsonUtil jsonUtil)
        {
            _jsonUtil = jsonUtil;
        }

        public IReadOnlyItemData GetItemData(Item item)
        {
            return _jsonUtil.DeserializeOrCreate<ItemData>(item.Data);
        }

        public void SetItemData(Item item, IReadOnlyItemData itemData)
        {
            item.Data = _jsonUtil.SerializeNonNull(itemData);
        }

        public bool IsItemEquipped(Item item)
        {
            return GetItemData(item).Equipped.HasValue;
        }

        public void UpdateEquipItem(Item item, int? level)
        {
            UpdateData(item, (itemData) =>
            {
                if (!itemData.Equipped.HasValue)
                    itemData.Equipped = 1;

                if (level == 1)
                {
                    itemData.Equipped += 1;
                    if (itemData.Equipped == 0)
                        itemData.Equipped = 1;
                }
                else if (level == -1)
                {
                    itemData.Equipped -= 1;
                    if (itemData.Equipped == 0)
                        itemData.Equipped = -1;
                }
                else if (level == 0)
                {
                    itemData.Equipped = null;
                }
            });
        }

        public void UpdateQuantity(Item item, int newQuantity)
        {
            UpdateData(item, itemData => itemData.Quantity = newQuantity);
        }

        public void UpdateRelativeQuantity(Item item, int quantityChange)
        {
            UpdateData(item, itemData => itemData.Quantity += quantityChange);
        }

        public void UpdateChargeCount(Item item, int newChargeCount)
        {
            UpdateData(item, itemData => itemData.Charge = newChargeCount);
        }

        public void UpdateRelativeChargeCount(Item item, int change)
        {
            UpdateData(item, itemData => itemData.Charge += change);
        }

        public void ResetReadCount(Item item)
        {
            UpdateData(item, (itemData) => itemData.ReadCount = 0);
        }

        private void UpdateData(Item item, Action<ItemData> action)
        {
            var itemData = _jsonUtil.DeserializeOrCreate<ItemData>(item.Data);
            action(itemData);
            SetItemData(item, itemData);
        }
    }
}