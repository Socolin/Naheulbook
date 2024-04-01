using System.Collections.Generic;
using System.Threading.Tasks;
using Naheulbook.Core.Exceptions;
using Naheulbook.Core.Factories;
using Naheulbook.Core.Models;
using Naheulbook.Core.Notifications;
using Naheulbook.Data.Factories;
using Naheulbook.Data.Models;
using Naheulbook.Shared.TransientModels;
using Naheulbook.Shared.Utils;

namespace Naheulbook.Core.Utils;

public interface IItemUtil
{
    void EquipItem(ItemEntity item, int? level);
    Task<(ItemEntity takenItem, int remainingQuantity)> MoveItemToAsync(int itemId, int characterId, int? quantity, MoveItemTrigger trigger);
    Task<IList<ItemEntity>> CreateInitialPlayerInventoryAsync(int money);
    bool DecrementQuantityOrDeleteItem(ItemEntity contextUsedItem);
}

public class ItemUtil(
    ICharacterHistoryUtil characterHistoryUtil,
    IItemDataUtil itemDataUtil,
    IJsonUtil jsonUtil,
    IUnitOfWorkFactory unitOfWorkFactory,
    INotificationSessionFactory notificationSessionFactory,
    IItemFactory itemFactory
) : IItemUtil
{
    public void EquipItem(ItemEntity item, int? level)
    {
        var previouslyEquipped = itemDataUtil.IsItemEquipped(item);
        itemDataUtil.UpdateEquipItem(item, level);
        var newlyEquipped = itemDataUtil.IsItemEquipped(item);

        if (previouslyEquipped != newlyEquipped && item.Character != null)
        {
            if (previouslyEquipped)
                item.Character.AddHistoryEntry(characterHistoryUtil.CreateLogUnEquipItem(item.Character.Id, item.Id));
            else
                item.Character.AddHistoryEntry(characterHistoryUtil.CreateLogEquipItem(item.Character.Id, item.Id));
        }
    }

    public async Task<(ItemEntity takenItem, int remainingQuantity)> MoveItemToAsync(int itemId, int characterId, int? quantity, MoveItemTrigger trigger)
    {
        var notificationSession = notificationSessionFactory.CreateSession();

        ItemEntity takenItem;
        var remainingQuantity = 0;

        using (var uow = unitOfWorkFactory.CreateUnitOfWork())
        {
            var targetCharacter = await uow.Characters.GetAsync(characterId);
            if (targetCharacter == null)
                throw new CharacterNotFoundException(characterId);
            var originalItem = await uow.Items.GetWithAllDataWithCharacterAsync(itemId);
            if (originalItem == null)
                throw new ItemNotFoundException(itemId);
            var originalItemData = itemDataUtil.GetItemData(originalItem);

            if (quantity == null || originalItemData.Quantity == null || quantity >= originalItemData.Quantity)
            {
                if (trigger == MoveItemTrigger.TakeItemFromLoot)
                    notificationSession.NotifyItemTakeItem(originalItem, targetCharacter, null);
                else if (trigger == MoveItemTrigger.GiveItem)
                {
                    notificationSession.NotifyItemDeleteItem(originalItem);
                    if (originalItem.CharacterId != null)
                        originalItem.Character!.AddHistoryEntry(characterHistoryUtil.CreateLogGiveItem(originalItem.CharacterId.Value, originalItem));
                }

                originalItem.Character = targetCharacter;
                originalItem.CharacterId = targetCharacter.Id;
                originalItem.MonsterId = null;
                originalItem.LootId = null;
                originalItem.ContainerId = null;
                itemDataUtil.ResetReadCount(originalItem);
                itemDataUtil.UnEquipItem(originalItem);

                takenItem = originalItem;

                notificationSession.NotifyCharacterAddItem(targetCharacter.Id, originalItem);
                if (trigger == MoveItemTrigger.TakeItemFromLoot)
                    targetCharacter.AddHistoryEntry(characterHistoryUtil.CreateLogLootItem(targetCharacter.Id, takenItem));
                else if (trigger == MoveItemTrigger.GiveItem)
                    targetCharacter.AddHistoryEntry(characterHistoryUtil.CreateLogGivenItem(targetCharacter.Id, takenItem));
            }
            else
            {
                var splitItem = SplitItem(originalItem, quantity.Value);
                uow.Items.Add(splitItem);
                splitItem.Character = targetCharacter;

                takenItem = splitItem;
                remainingQuantity = itemDataUtil.GetItemData(originalItem).Quantity!.Value;

                if (trigger == MoveItemTrigger.TakeItemFromLoot)
                {
                    targetCharacter.AddHistoryEntry(characterHistoryUtil.CreateLogLootItem(targetCharacter.Id, takenItem));
                    notificationSession.NotifyItemTakeItem(originalItem, targetCharacter, remainingQuantity);
                }
                else if (trigger == MoveItemTrigger.GiveItem)
                {
                    if (originalItem.CharacterId != null)
                        originalItem.Character!.AddHistoryEntry(characterHistoryUtil.CreateLogGiveItem(originalItem.CharacterId.Value, originalItem));
                    targetCharacter.AddHistoryEntry(characterHistoryUtil.CreateLogGivenItem(targetCharacter.Id, takenItem));
                    notificationSession.NotifyItemDataChanged(originalItem);
                }
                notificationSession.NotifyCharacterAddItem(targetCharacter.Id, splitItem, true);
            }

            await uow.SaveChangesAsync();
        }

        await notificationSession.CommitAsync();

        return (takenItem, remainingQuantity);
    }

    public async Task<IList<ItemEntity>> CreateInitialPlayerInventoryAsync(int money)
    {
        ItemTemplateEntity purseItemTemplate;
        ItemTemplateEntity goldCoinItemTemplate;

        using (var uow = unitOfWorkFactory.CreateUnitOfWork())
        {
            purseItemTemplate = await uow.ItemTemplates.GetPurseItemTemplateBasedOnMoneyAsync(money);
            goldCoinItemTemplate = await uow.ItemTemplates.GetGoldCoinItemTemplate();
        }

        var purseItem = itemFactory.CreateItem(purseItemTemplate, new ItemData {Equipped = 1});
        var moneyItem = itemFactory.CreateItem(goldCoinItemTemplate, new ItemData {Quantity = money});
        moneyItem.Container = purseItem;

        return new List<ItemEntity> {moneyItem, purseItem};
    }

    public bool DecrementQuantityOrDeleteItem(ItemEntity item)
    {
        var itemData = itemDataUtil.GetItemData(item);
        if (itemData.Quantity <= 1)
            return true;

        var itemTemplateData = jsonUtil.DeserializeOrCreate<ItemTemplateData>(item.ItemTemplate.Data);
        if (itemTemplateData.Charge.HasValue)
            itemDataUtil.UpdateChargeCount(item, itemTemplateData.Charge.Value);

        itemDataUtil.UpdateRelativeQuantity(item, -1);
        itemDataUtil.SetItemData(item, itemData);

        return false;
    }

    private ItemEntity SplitItem(ItemEntity originalItem, int quantity)
    {
        var splitItem = itemFactory.CloneItem(originalItem);

        itemDataUtil.UpdateQuantity(splitItem, quantity);
        itemDataUtil.UpdateRelativeQuantity(originalItem, -quantity);

        return splitItem;
    }
}