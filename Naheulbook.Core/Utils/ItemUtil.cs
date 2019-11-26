using System.Collections.Generic;
using System.Threading.Tasks;
using Naheulbook.Core.Factories;
using Naheulbook.Core.Models;
using Naheulbook.Core.Notifications;
using Naheulbook.Data.Factories;
using Naheulbook.Data.Models;
using Naheulbook.Shared.TransientModels;
using Naheulbook.Shared.Utils;

namespace Naheulbook.Core.Utils
{
    public interface IItemUtil
    {
        void EquipItem(Item item, int? level);
        Task<(Item takenItem, int remainingQuantity)> MoveItemToAsync(int itemId, int characterId, int? quantity, MoveItemTrigger trigger);
        Task<IList<Item>> CreateInitialPlayerInventoryAsync(int money);
        bool DecrementQuantityOrDeleteItem(Item contextUsedItem);
    }

    public class ItemUtil : IItemUtil
    {
        private readonly IItemDataUtil _itemDataUtil;
        private readonly ICharacterHistoryUtil _characterHistoryUtil;
        private readonly IJsonUtil _jsonUtil;
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly INotificationSessionFactory _notificationSessionFactory;
        private readonly IItemFactory _itemFactory;

        public ItemUtil(
            ICharacterHistoryUtil characterHistoryUtil,
            IItemDataUtil itemDataUtil,
            IJsonUtil jsonUtil,
            IUnitOfWorkFactory unitOfWorkFactory,
            INotificationSessionFactory notificationSessionFactory,
            IItemFactory itemFactory
        )
        {
            _characterHistoryUtil = characterHistoryUtil;
            _itemDataUtil = itemDataUtil;
            _jsonUtil = jsonUtil;
            _unitOfWorkFactory = unitOfWorkFactory;
            _notificationSessionFactory = notificationSessionFactory;
            _itemFactory = itemFactory;
        }

        public void EquipItem(Item item, int? level)
        {
            var previouslyEquipped = _itemDataUtil.IsItemEquipped(item);
            _itemDataUtil.UpdateEquipItem(item, level);
            var newlyEquipped = _itemDataUtil.IsItemEquipped(item);

            if (previouslyEquipped != newlyEquipped && item.CharacterId != null)
            {
                if (item.Character!.HistoryEntries == null)
                    item.Character!.HistoryEntries = new List<CharacterHistoryEntry>();
                if (previouslyEquipped)
                    item.Character!.HistoryEntries.Add(_characterHistoryUtil.CreateLogUnEquipItem(item.CharacterId.Value, item.Id));
                else
                    item.Character!.HistoryEntries.Add(_characterHistoryUtil.CreateLogEquipItem(item.CharacterId.Value, item.Id));
            }
        }

        public async Task<(Item takenItem, int remainingQuantity)> MoveItemToAsync(int itemId, int characterId, int? quantity, MoveItemTrigger trigger)
        {
            var notificationSession = _notificationSessionFactory.CreateSession();

            Item takenItem;
            var remainingQuantity = 0;

            using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
            {
                var targetCharacter = await uow.Characters.GetAsync(characterId);
                var originalItem = await uow.Items.GetWithAllDataWithCharacterAsync(itemId);
                var originalItemData = _itemDataUtil.GetItemData(originalItem);

                if (quantity == null || originalItemData.Quantity == null || quantity >= originalItemData.Quantity)
                {
                    if (trigger == MoveItemTrigger.TakeItemFromLoot)
                        notificationSession.NotifyItemTakeItem(originalItem, targetCharacter, null);
                    else if (trigger == MoveItemTrigger.GiveItem)
                    {
                        notificationSession.NotifyItemDeleteItem(originalItem);
                        if (originalItem.CharacterId != null)
                            originalItem.Character!.AddHistoryEntry(_characterHistoryUtil.CreateLogGiveItem(originalItem.CharacterId.Value, originalItem));
                    }

                    originalItem.Character = targetCharacter;
                    originalItem.MonsterId = null;
                    originalItem.LootId = null;
                    originalItem.ContainerId = null;
                    _itemDataUtil.ResetReadCount(originalItem);
                    _itemDataUtil.UnEquipItem(originalItem);

                    takenItem = originalItem;

                    notificationSession.NotifyCharacterAddItem(targetCharacter.Id, originalItem);
                    if (trigger == MoveItemTrigger.TakeItemFromLoot)
                        targetCharacter.AddHistoryEntry(_characterHistoryUtil.CreateLogLootItem(targetCharacter.Id, takenItem));
                    else if (trigger == MoveItemTrigger.GiveItem)
                        targetCharacter.AddHistoryEntry(_characterHistoryUtil.CreateLogGivenItem(targetCharacter.Id, takenItem));
                }
                else
                {
                    var splitItem = SplitItem(originalItem, quantity.Value);
                    splitItem.Character = targetCharacter;
                    uow.Items.Add(splitItem);

                    takenItem = splitItem;
                    remainingQuantity = originalItemData.Quantity.Value;

                    if (trigger == MoveItemTrigger.TakeItemFromLoot)
                    {
                        targetCharacter.AddHistoryEntry(_characterHistoryUtil.CreateLogLootItem(targetCharacter.Id, takenItem));
                        notificationSession.NotifyItemTakeItem(originalItem, targetCharacter, remainingQuantity);
                    }
                    else if (trigger == MoveItemTrigger.GiveItem)
                    {
                        if (originalItem.CharacterId != null)
                            originalItem.Character!.AddHistoryEntry(_characterHistoryUtil.CreateLogGiveItem(originalItem.CharacterId.Value, originalItem));
                        targetCharacter.AddHistoryEntry(_characterHistoryUtil.CreateLogGivenItem(targetCharacter.Id, takenItem));
                        notificationSession.NotifyItemDataChanged(originalItem);
                    }
                    notificationSession.NotifyCharacterAddItem(targetCharacter.Id, splitItem);
                }

                await uow.SaveChangesAsync();
            }

            await notificationSession.CommitAsync();

            return (takenItem, remainingQuantity);
        }

        public async Task<IList<Item>> CreateInitialPlayerInventoryAsync(int money)
        {
            ItemTemplate purseItemTemplate;
            ItemTemplate goldCoinItemTemplate;

            using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
            {
                purseItemTemplate = await uow.ItemTemplates.GetPurseItemTemplateBasedOnMoneyAsync(money);
                goldCoinItemTemplate = await uow.ItemTemplates.GetGoldCoinItemTemplate();
            }

            var purseItem = _itemFactory.CreateItem(purseItemTemplate, new ItemData {Equipped = 1});
            var moneyItem = _itemFactory.CreateItem(goldCoinItemTemplate, new ItemData {Quantity = money});
            moneyItem.Container = purseItem;

            return new List<Item> {moneyItem, purseItem};
        }

        public bool DecrementQuantityOrDeleteItem(Item item)
        {
            var itemData = _itemDataUtil.GetItemData(item);
            if (itemData.Quantity <= 1)
                return true;

            var itemTemplateData = _jsonUtil.DeserializeOrCreate<ItemTemplateData>(item.ItemTemplate.Data);
            if (itemTemplateData.Charge.HasValue)
                _itemDataUtil.UpdateChargeCount(item, itemTemplateData.Charge.Value);

            _itemDataUtil.UpdateRelativeQuantity(item, -1);
            _itemDataUtil.SetItemData(item, itemData);

            return false;
        }

        private Item SplitItem(Item originalItem, int quantity)
        {
            var splitItem = _itemFactory.CloneItem(originalItem);

            _itemDataUtil.UpdateQuantity(splitItem, quantity);
            _itemDataUtil.UpdateRelativeQuantity(splitItem, -quantity);

            return splitItem;
        }
    }
}