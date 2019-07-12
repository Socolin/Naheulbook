using System.Collections.Generic;
using System.Threading.Tasks;
using Naheulbook.Core.Models;
using Naheulbook.Data.Models;
using Naheulbook.Requests.Requests;
using Naheulbook.Shared.TransientModels;

namespace Naheulbook.Core.Notifications
{
    public interface INotificationSession
    {
        void NotifyCharacterChangeEv(Character character);
        void NotifyCharacterChangeEa(Character character);
        void NotifyCharacterChangeFatePoint(Character character);
        void NotifyCharacterChangeExperience(Character character);
        void NotifyCharacterChangeSex(Character character);
        void NotifyCharacterChangeName(Character character);
        void NotifyCharacterAddItem(int characterId, Item item);
        void NotifyCharacterSetStatBonusAd(int characterId, string stat);
        void NotifyCharacterAddModifier(int characterId, CharacterModifier characterModifier);
        void NotifyCharacterRemoveModifier(int characterId, int characterModifierId);
        void NotifyCharacterUpdateModifier(int characterId, CharacterModifier characterModifier);
        void NotifyCharacterGroupInvite(int characterId, GroupInvite groupInvite);
        void NotifyCharacterCancelGroupInvite(int characterId, GroupInvite groupInvite);
        void NotifyCharacterAcceptGroupInvite(int characterId, GroupInvite groupInvite);
        void NotifyCharacterShowLoot(int characterId, Loot loot);
        void NotifyCharacterHideLoot(int characterId, int lootId);

        void NotifyCharacterGmChangeColor(Character character);
        void NotifyCharacterGmChangeTarget(Character character, TargetRequest requestTarget);
        void NotifyCharacterGmChangeData(Character character, CharacterGmData gmData);
        void NotifyCharacterGmChangeActive(Character character);

        void NotifyItemDataChanged(Item item);
        void NotifyItemModifiersChanged(Item item);
        void NotifyEquipItem(Item item);
        void NotifyItemChangeContainer(Item item);
        void NotifyItemUpdateModifier(Item item);
        void NotifyItemDeleteItem(Item item);
        void NotifyItemTakeItem(Item item, Character character, int? remainingQuantity);

        void NotifyGroupCharacterInvite(int groupId, GroupInvite groupInvite);
        void NotifyGroupCancelGroupInvite(int groupId, GroupInvite groupInvite);
        void NotifyGroupAcceptGroupInvite(int groupId, GroupInvite groupInvite);
        void NotifyGroupChangeGroupData(int groupId, GroupData groupData);
        void NotifyGroupChangeLocation(int groupId, Location location);
        void NotifyGroupAddLoot(int groupId, Loot loot);
        void NotifyGroupDeleteLoot(int groupId, int lootId);

        void NotifyLootUpdateVisibility(int lootId, bool visibleForPlayer);
        void NotifyLootAddMonster(int lootId, Monster monster);
        void NotifyLootAddItem(int lootId, Item item);

        void NotifyMonsterAddModifier(int monsterId, ActiveStatsModifier modifier);
        void NotifyMonsterUpdateModifier(int monsterId, ActiveStatsModifier modifier);
        void NotifyMonsterRemoveModifier(int monsterId, int modifierId);
        void NotifyMonsterAddItem(int monsterId, Item item);

        Task CommitAsync();
    }

    public class NotificationSession : INotificationSession
    {
        private readonly IList<INotificationPacket> _packets = new List<INotificationPacket>();
        private readonly INotificationPacketBuilder _packetBuilder;
        private readonly INotificationSender _notificationSender;

        public NotificationSession(
            INotificationPacketBuilder packetBuilder,
            INotificationSender notificationSender
        )
        {
            _packetBuilder = packetBuilder;
            _notificationSender = notificationSender;
        }

        public void NotifyCharacterChangeEv(Character character)
        {
            _packets.Add(_packetBuilder.BuildCharacterChangeEv(character));
        }

        public void NotifyCharacterChangeEa(Character character)
        {
            _packets.Add(_packetBuilder.BuildCharacterChangeEa(character));
        }

        public void NotifyCharacterChangeFatePoint(Character character)
        {
            _packets.Add(_packetBuilder.BuildCharacterChangeFatePoint(character));
        }

        public void NotifyCharacterChangeExperience(Character character)
        {
            _packets.Add(_packetBuilder.BuildCharacterChangeExperience(character));
        }

        public void NotifyCharacterChangeSex(Character character)
        {
            _packets.Add(_packetBuilder.BuildCharacterChangeSex(character));
        }

        public void NotifyCharacterChangeName(Character character)
        {
            _packets.Add(_packetBuilder.BuildCharacterChangeName(character));
        }

        public void NotifyCharacterAddItem(int characterId, Item item)
        {
            _packets.Add(_packetBuilder.BuildCharacterAddItem(characterId, item));
        }

        public void NotifyCharacterUpdateModifier(int characterId, CharacterModifier modifier)
        {
            _packets.Add(_packetBuilder.BuildCharacterUpdateModifier(characterId, modifier));
        }

        public void NotifyCharacterGmChangeColor(Character character)
        {
            _packets.Add(_packetBuilder.BuildCharacterChangeColor(character));
        }

        public void NotifyCharacterGmChangeTarget(Character character, TargetRequest targetInfo)
        {
            _packets.Add(_packetBuilder.BuildCharacterChangeTarget(character, targetInfo));
        }

        public void NotifyCharacterGmChangeData(Character character, CharacterGmData gmData)
        {
            _packets.Add(_packetBuilder.BuildCharacterChangeData(character, gmData));
        }

        public void NotifyCharacterGmChangeActive(Character character)
        {
            _packets.Add(_packetBuilder.BuildCharacterChangeActive(character));
        }

        public void NotifyItemDataChanged(Item item)
        {
            _packets.Add(_packetBuilder.BuildItemDataChanged(item));
        }

        public void NotifyItemModifiersChanged(Item item)
        {
            _packets.Add(_packetBuilder.BuildItemModifiersChanged(item));
        }

        public void NotifyEquipItem(Item item)
        {
            _packets.Add(_packetBuilder.BuildEquipItem(item));
        }

        public void NotifyItemChangeContainer(Item item)
        {
            _packets.Add(_packetBuilder.BuildItemChangeContainer(item));
        }

        public void NotifyItemUpdateModifier(Item item)
        {
            _packets.Add(_packetBuilder.BuildItemUpdateModifier(item));
        }

        public void NotifyItemDeleteItem(Item item)
        {
            _packets.Add(_packetBuilder.BuildItemDeleteItem(item));
        }

        public void NotifyItemTakeItem(Item item, Character character, int? remainingQuantity)
        {
            _packets.Add(_packetBuilder.BuildItemTakeItem(item, character, remainingQuantity));
        }

        public void NotifyCharacterSetStatBonusAd(int characterId, string stat)
        {
            _packets.Add(_packetBuilder.BuildCharacterSetStatBonusAd(characterId, stat));
        }

        public void NotifyCharacterAddModifier(int characterId, CharacterModifier characterModifier)
        {
            _packets.Add(_packetBuilder.BuildCharacterAddModifier(characterId, characterModifier));
        }

        public void NotifyCharacterRemoveModifier(int characterId, int characterModifierId)
        {
            _packets.Add(_packetBuilder.BuildCharacterRemoveModifier(characterId, characterModifierId));
        }

        public void NotifyCharacterGroupInvite(int characterId, GroupInvite groupInvite)
        {
            _packets.Add(_packetBuilder.BuildCharacterGroupInvite(characterId, groupInvite));
        }

        public void NotifyCharacterCancelGroupInvite(int characterId, GroupInvite groupInvite)
        {
            _packets.Add(_packetBuilder.BuildCharacterCancelGroupInvite(characterId, groupInvite));
        }

        public void NotifyCharacterAcceptGroupInvite(int characterId, GroupInvite groupInvite)
        {
            _packets.Add(_packetBuilder.BuildCharacterAcceptGroupInvite(characterId, groupInvite));
        }

        public void NotifyCharacterShowLoot(int characterId, Loot loot)
        {
            _packets.Add(_packetBuilder.BuildCharacterShowLoot(characterId, loot));
        }

        public void NotifyCharacterHideLoot(int characterId, int lootId)
        {
            _packets.Add(_packetBuilder.BuildCharacterHideLoot(characterId, lootId));
        }

        public void NotifyGroupCharacterInvite(int groupId, GroupInvite groupInvite)
        {
            _packets.Add(_packetBuilder.BuildGroupCharacterInvite(groupId, groupInvite));
        }

        public void NotifyGroupCancelGroupInvite(int groupId, GroupInvite groupInvite)
        {
            _packets.Add(_packetBuilder.BuildGroupCancelGroupInvite(groupId, groupInvite));
        }

        public void NotifyGroupAcceptGroupInvite(int groupId, GroupInvite groupInvite)
        {
            _packets.Add(_packetBuilder.BuildGroupAcceptGroupInvite(groupId, groupInvite));
        }

        public void NotifyGroupChangeGroupData(int groupId, GroupData groupData)
        {
            _packets.Add(_packetBuilder.BuildGroupChangeGroupData(groupId, groupData));
        }

        public void NotifyGroupChangeLocation(int groupId, Location location)
        {
            _packets.Add(_packetBuilder.BuildGroupChangeLocation(groupId, location));
        }

        public void NotifyGroupAddLoot(int groupId, Loot loot)
        {
            _packets.Add(_packetBuilder.BuildGroupAddLoot(groupId, loot));
        }

        public void NotifyGroupDeleteLoot(int groupId, int lootId)
        {
            _packets.Add(_packetBuilder.BuildGroupDeleteLoot(groupId, lootId));
        }

        public void NotifyLootUpdateVisibility(int lootId, bool visibleForPlayer)
        {
            _packets.Add(_packetBuilder.BuildLootUpdateVisibility(lootId, visibleForPlayer));
        }

        public void NotifyLootAddMonster(int lootId, Monster monster)
        {
            _packets.Add(_packetBuilder.BuildLootAddMonster(lootId, monster));
        }

        public void NotifyLootAddItem(int lootId, Item item)
        {
            _packets.Add(_packetBuilder.BuildLootAddItem(lootId, item));
        }

        public void NotifyMonsterAddModifier(int monsterId, ActiveStatsModifier modifier)
        {
            _packets.Add(_packetBuilder.BuildMonsterAddModifier(monsterId, modifier));
        }

        public void NotifyMonsterUpdateModifier(int monsterId, ActiveStatsModifier modifier)
        {
            _packets.Add(_packetBuilder.BuildMonsterUpdateModifier(monsterId, modifier));
        }

        public void NotifyMonsterRemoveModifier(int monsterId, int modifierId)
        {
            _packets.Add(_packetBuilder.BuildMonsterRemoveModifier(monsterId, modifierId));
        }

        public void NotifyMonsterAddItem(int monsterId, Item item)
        {
            _packets.Add(_packetBuilder.BuildMonsterAddItem(monsterId, item));
        }

        public async Task CommitAsync()
        {
            await _notificationSender.SendPacketsAsync(_packets);
            _packets.Clear();
        }
    }
}