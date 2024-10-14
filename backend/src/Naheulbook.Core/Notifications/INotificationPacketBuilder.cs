using System;
using Naheulbook.Core.Models;
using Naheulbook.Data.Models;
using Naheulbook.Requests.Requests;
using Naheulbook.Shared.TransientModels;

namespace Naheulbook.Core.Notifications;

public interface INotificationPacketBuilder
{
    INotificationPacket BuildCharacterChangeEv(CharacterEntity character);
    INotificationPacket BuildCharacterChangeEa(CharacterEntity character);
    INotificationPacket BuildCharacterChangeFatePoint(CharacterEntity character);
    INotificationPacket BuildCharacterChangeExperience(CharacterEntity character);
    INotificationPacket BuildCharacterChangeSex(CharacterEntity character);
    INotificationPacket BuildCharacterChangeName(CharacterEntity character);
    INotificationPacket BuildCharacterChangeNotes(CharacterEntity character);
    INotificationPacket BuildCharacterAddItem(int characterId, ItemEntity item);
    INotificationPacket BuildCharacterSetStatBonusAd(int characterId, string stat);
    INotificationPacket BuildCharacterAddModifier(int characterId, CharacterModifierEntity characterModifier);
    INotificationPacket BuildCharacterRemoveModifier(int characterId, int characterModifierId);
    INotificationPacket BuildCharacterUpdateModifier(int characterId, CharacterModifierEntity modifier);
    INotificationPacket BuildCharacterGroupInvite(int characterId, GroupInviteEntity groupInvite);
    INotificationPacket BuildCharacterCancelGroupInvite(int characterId, GroupInviteEntity groupInvite);
    INotificationPacket BuildCharacterAcceptGroupInvite(int characterId, GroupInviteEntity groupInvite);
    INotificationPacket BuildCharacterShowLoot(int characterId, LootEntity loot);
    INotificationPacket BuildCharacterHideLoot(int characterId, int lootId);
    INotificationPacket BuildCharacterLevelUp(int characterId, LevelUpResult levelUpResult);
    INotificationPacket BuildCharacterAddJob(int characterId, Guid jobId);
    INotificationPacket BuildCharacterRemoveJob(int characterId, Guid jobId);

    INotificationPacket BuildCharacterChangeColor(CharacterEntity character);
    INotificationPacket BuildCharacterChangeTarget(CharacterEntity character, TargetRequest requestTarget);
    INotificationPacket BuildCharacterChangeData(CharacterEntity character, CharacterGmData gmData);
    INotificationPacket BuildCharacterChangeActive(CharacterEntity character);

    INotificationPacket BuildItemDataChanged(ItemEntity item);
    INotificationPacket BuildItemModifiersChanged(ItemEntity item);
    INotificationPacket BuildEquipItem(ItemEntity item);
    INotificationPacket BuildItemChangeContainer(ItemEntity item);
    INotificationPacket BuildItemUpdateModifier(ItemEntity item);
    INotificationPacket BuildItemDeleteItem(ItemEntity item);
    INotificationPacket BuildItemTakeItem(ItemEntity item, CharacterEntity character, int? takenQuantity);

    INotificationPacket BuildGroupCharacterInvite(int groupId, GroupInviteEntity groupInvite);
    INotificationPacket BuildGroupCancelGroupInvite(int groupId, GroupInviteEntity groupInvite);
    INotificationPacket BuildGroupAcceptGroupInvite(int groupId, GroupInviteEntity groupInvite);
    INotificationPacket BuildGroupChangeGroupData(int groupId, GroupData groupData);
    INotificationPacket BuildGroupAddLoot(int groupId, LootEntity loot);
    INotificationPacket BuildGroupDeleteLoot(int groupId, int lootId);
    INotificationPacket BuildGroupAddFight(int groupId, FightEntity fight);
    INotificationPacket BuildGroupDeleteFight(int groupId, int fightId);
    INotificationPacket BuildGroupChangeConfig(in int groupId, GroupConfig groupConfig);
    INotificationPacket BuildGroupAddMonster(int groupId, MonsterEntity monster);
    INotificationPacket BuildGroupKillMonster(int monsterGroupId, int monsterId);
    INotificationPacket BuildGroupDeleteMonster(int groupId, int monsterId);

    INotificationPacket BuildLootUpdateVisibility(int lootId, bool visibleForPlayer);
    INotificationPacket BuildLootAddMonster(int lootId, MonsterEntity monster);
    INotificationPacket BuildLootAddItem(int lootId, ItemEntity item);
    INotificationPacket BuildLootDeleteMonster(int lootId, int monsterId);

    INotificationPacket BuildMonsterAddModifier(int monsterId, ActiveStatsModifier modifier);
    INotificationPacket BuildMonsterUpdateModifier(int monsterId, ActiveStatsModifier modifier);
    INotificationPacket BuildMonsterRemoveModifier(int monsterId, int modifierId);
    INotificationPacket BuildMonsterAddItem(int monsterId, ItemEntity item);
    INotificationPacket BuildMonsterUpdateData(int monsterId, MonsterData monsterData);
    INotificationPacket BuildMonsterChangeTarget(int monsterId, TargetRequest targetInfo);
    INotificationPacket BuildMonsterChangeName(int monsterId, string name);

    INotificationPacket BuildFightAddMonster(int fightId, MonsterEntity monster);
    INotificationPacket BuildFightRemoveMonster(int fightId, int monsterId);
}