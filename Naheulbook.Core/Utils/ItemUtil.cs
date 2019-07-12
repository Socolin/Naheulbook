using System.Collections.Generic;
using System.Threading.Tasks;
using Naheulbook.Core.Models;
using Naheulbook.Core.Notifications;
using Naheulbook.Data.Factories;
using Naheulbook.Data.Models;
using Naheulbook.Shared.Utils;
using Newtonsoft.Json.Linq;

namespace Naheulbook.Core.Utils
{
    public interface IItemUtil
    {
        void EquipItem(Item item, int? level);
        Task<(Item takenItem, int remainingQuantity)> MoveItemToAsync(int itemId, int characterId, int? quantity);
    }

    public class ItemUtil : IItemUtil
    {
        private readonly IItemDataUtil _itemDataUtil;
        private readonly ICharacterHistoryUtil _characterHistoryUtil;
        private readonly IJsonUtil _jsonUtil;
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly INotificationSessionFactory _notificationSessionFactory;

        public ItemUtil(
            ICharacterHistoryUtil characterHistoryUtil,
            IItemDataUtil itemDataUtil,
            IJsonUtil jsonUtil,
            IUnitOfWorkFactory unitOfWorkFactory,
            INotificationSessionFactory notificationSessionFactory
        )
        {
            _characterHistoryUtil = characterHistoryUtil;
            _itemDataUtil = itemDataUtil;
            _jsonUtil = jsonUtil;
            _unitOfWorkFactory = unitOfWorkFactory;
            _notificationSessionFactory = notificationSessionFactory;
        }

        public void EquipItem(Item item, int? level)
        {
            var itemData = _jsonUtil.Deserialize<JObject>(item.Data);

            var previouslyEquipped = _itemDataUtil.IsItemEquipped(itemData);
            _itemDataUtil.UpdateEquipItem(itemData, level);
            var newlyEquipped = _itemDataUtil.IsItemEquipped(itemData);

            if (previouslyEquipped != newlyEquipped && item.CharacterId != null)
            {
                if (item.Character.HistoryEntries == null)
                    item.Character.HistoryEntries = new List<CharacterHistoryEntry>();
                if (previouslyEquipped)
                    item.Character.HistoryEntries.Add(_characterHistoryUtil.CreateLogUnEquipItem(item.CharacterId.Value, item.Id));
                else
                    item.Character.HistoryEntries.Add(_characterHistoryUtil.CreateLogEquipItem(item.CharacterId.Value, item.Id));
            }

            item.Data = _jsonUtil.Serialize(itemData);
        }

        public async Task<(Item takenItem, int remainingQuantity)> MoveItemToAsync(int itemId, int characterId, int? quantity)
        {
            var notificationSession = _notificationSessionFactory.CreateSession();

            Item takenItem = null;
            var remainingQuantity = 0;

            using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
            {
                var targetCharacter = await uow.Characters.GetAsync(characterId);
                var originalItem = await uow.Items.GetWithAllDataAsync(itemId);
                var originalItemData = _jsonUtil.Deserialize<ItemData>(originalItem.Data) ?? new ItemData();

                if (quantity == null || originalItemData.Quantity == null || quantity >= originalItemData.Quantity)
                {
                    notificationSession.NotifyItemTakeItem(originalItem, targetCharacter, null);

                    originalItem.Character = targetCharacter;
                    originalItem.MonsterId = null;
                    originalItem.LootId = null;

                    takenItem = originalItem;

                    notificationSession.NotifyCharacterAddItem(targetCharacter.Id, originalItem);
                }
                else
                {
                    var splitItem = SplitItem(originalItem, originalItemData, quantity.Value);
                    splitItem.Character = targetCharacter;
                    uow.Items.Add(splitItem);

                    takenItem = splitItem;
                    remainingQuantity = originalItemData.Quantity.Value;

                    notificationSession.NotifyItemTakeItem(originalItem, targetCharacter, remainingQuantity);
                    notificationSession.NotifyCharacterAddItem(targetCharacter.Id, splitItem);
                }

                await uow.CompleteAsync();
            }

            await notificationSession.CommitAsync();

            return (takenItem, remainingQuantity);
        }

        private Item SplitItem(Item originalItem, ItemData originalItemData, int quantity)
        {
            originalItemData.Quantity -= quantity;
            originalItem.Data = _jsonUtil.Serialize(originalItemData);

            var originalQuantity = originalItemData.Quantity;
            originalItemData.Quantity = quantity;
            var splitItem = new Item
            {
                Modifiers = originalItem.Modifiers,
                ItemTemplateId = originalItem.ItemTemplateId,
                Data = _jsonUtil.Serialize(originalItemData)
            };

            originalItemData.Quantity = originalQuantity;

            return splitItem;
        }
    }
}