using System;
using Naheulbook.Core.Models;
using Naheulbook.Data.Models;
using Naheulbook.Requests.Requests;
using Naheulbook.Shared.TransientModels;

namespace Naheulbook.Core.Notifications
{
    public interface INotificationPacketBuilder
    {
        INotificationPacket BuildCharacterChangeEv(Character character);
        INotificationPacket BuildCharacterChangeEa(Character character);
        INotificationPacket BuildCharacterChangeFatePoint(Character character);
        INotificationPacket BuildCharacterChangeExperience(Character character);
        INotificationPacket BuildCharacterChangeSex(Character character);
        INotificationPacket BuildCharacterChangeName(Character character);
        INotificationPacket BuildCharacterChangeNotes(Character character);
        INotificationPacket BuildCharacterAddItem(int characterId, Item item);
        INotificationPacket BuildCharacterSetStatBonusAd(int characterId, string stat);
        INotificationPacket BuildCharacterAddModifier(int characterId, CharacterModifier characterModifier);
        INotificationPacket BuildCharacterRemoveModifier(int characterId, int characterModifierId);
        INotificationPacket BuildCharacterUpdateModifier(int characterId, CharacterModifier modifier);
        INotificationPacket BuildCharacterGroupInvite(int characterId, GroupInvite groupInvite);
        INotificationPacket BuildCharacterCancelGroupInvite(int characterId, GroupInvite groupInvite);
        INotificationPacket BuildCharacterAcceptGroupInvite(int characterId, GroupInvite groupInvite);
        INotificationPacket BuildCharacterShowLoot(int characterId, Loot loot);
        INotificationPacket BuildCharacterHideLoot(int characterId, int lootId);
        INotificationPacket BuildCharacterLevelUp(int characterId, LevelUpResult levelUpResult);
        INotificationPacket BuildCharacterAddJob(int characterId, Guid jobId);
        INotificationPacket BuildCharacterRemoveJob(int characterId, Guid jobId);

        INotificationPacket BuildCharacterChangeColor(Character character);
        INotificationPacket BuildCharacterChangeTarget(Character character, TargetRequest requestTarget);
        INotificationPacket BuildCharacterChangeData(Character character, CharacterGmData gmData);
        INotificationPacket BuildCharacterChangeActive(Character character);

        INotificationPacket BuildItemDataChanged(Item item);
        INotificationPacket BuildItemModifiersChanged(Item item);
        INotificationPacket BuildEquipItem(Item item);
        INotificationPacket BuildItemChangeContainer(Item item);
        INotificationPacket BuildItemUpdateModifier(Item item);
        INotificationPacket BuildItemDeleteItem(Item item);
        INotificationPacket BuildItemTakeItem(Item item, Character character, int? takenQuantity);

        INotificationPacket BuildGroupCharacterInvite(int groupId, GroupInvite groupInvite);
        INotificationPacket BuildGroupCancelGroupInvite(int groupId, GroupInvite groupInvite);
        INotificationPacket BuildGroupAcceptGroupInvite(int groupId, GroupInvite groupInvite);
        INotificationPacket BuildGroupChangeGroupData(int groupId, GroupData groupData);
        INotificationPacket BuildGroupAddLoot(int groupId, Loot loot);
        INotificationPacket BuildGroupDeleteLoot(int groupId, int lootId);
        INotificationPacket BuildGroupChangeConfig(in int groupId, GroupConfig groupConfig);
        INotificationPacket BuildGroupAddMonster(int groupId, Monster monster);
        INotificationPacket BuildGroupKillMonster(int monsterGroupId, int monsterId);

        INotificationPacket BuildLootUpdateVisibility(int lootId, bool visibleForPlayer);
        INotificationPacket BuildLootAddMonster(int lootId, Monster monster);
        INotificationPacket BuildLootAddItem(int lootId, Item item);

        INotificationPacket BuildMonsterAddModifier(int monsterId, ActiveStatsModifier modifier);
        INotificationPacket BuildMonsterUpdateModifier(int monsterId, ActiveStatsModifier modifier);
        INotificationPacket BuildMonsterRemoveModifier(int monsterId, int modifierId);
        INotificationPacket BuildMonsterAddItem(int monsterId, Item item);
        INotificationPacket BuildMonsterUpdateData(int monsterId, MonsterData monsterData);
        INotificationPacket BuildMonsterChangeTarget(int monsterId, TargetRequest targetInfo);
        INotificationPacket BuildMonsterChangeName(int monsterId, string name);
    }
}