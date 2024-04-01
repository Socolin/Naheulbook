using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Naheulbook.Core.Models;
using Naheulbook.Data.Models;
using Naheulbook.Requests.Requests;
using Naheulbook.Shared.TransientModels;

namespace Naheulbook.Core.Notifications;

public interface INotificationSession
{
    void NotifyCharacterChangeEv(CharacterEntity character);
    void NotifyCharacterChangeEa(CharacterEntity character);
    void NotifyCharacterChangeFatePoint(CharacterEntity character);
    void NotifyCharacterChangeExperience(CharacterEntity character);
    void NotifyCharacterChangeSex(CharacterEntity character);
    void NotifyCharacterChangeName(CharacterEntity character);
    void NotifyCharacterChangeNotes(CharacterEntity character);
    void NotifyCharacterAddItem(int characterId, ItemEntity item, bool delayBuildPacket = false);
    void NotifyCharacterSetStatBonusAd(int characterId, string stat);
    void NotifyCharacterAddModifier(int characterId, CharacterModifierEntity characterModifier, bool delayBuildPacket = false);
    void NotifyCharacterRemoveModifier(int characterId, int characterModifierId);
    void NotifyCharacterUpdateModifier(int characterId, CharacterModifierEntity characterModifier);
    void NotifyCharacterGroupInvite(int characterId, GroupInviteEntity groupInvite);
    void NotifyCharacterCancelGroupInvite(int characterId, GroupInviteEntity groupInvite);
    void NotifyCharacterAcceptGroupInvite(int characterId, GroupInviteEntity groupInvite);
    void NotifyCharacterShowLoot(int characterId, LootEntity loot);
    void NotifyCharacterHideLoot(int characterId, int lootId);
    void NotifyCharacterLevelUp(int characterId, LevelUpResult levelUpResult);
    void NotifyCharacterAddJob(int characterId, Guid jobId);
    void NotifyCharacterRemoveJob(int characterId, Guid jobId);

    void NotifyCharacterGmChangeColor(CharacterEntity character);
    void NotifyCharacterGmChangeTarget(CharacterEntity character, TargetRequest requestTarget);
    void NotifyCharacterGmChangeData(CharacterEntity character, CharacterGmData gmData);
    void NotifyCharacterGmChangeActive(CharacterEntity character);

    void NotifyItemDataChanged(ItemEntity item);
    void NotifyItemModifiersChanged(ItemEntity item);
    void NotifyEquipItem(ItemEntity item);
    void NotifyItemChangeContainer(ItemEntity item);
    void NotifyItemUpdateModifier(ItemEntity item);
    void NotifyItemDeleteItem(ItemEntity item);
    void NotifyItemTakeItem(ItemEntity item, CharacterEntity character, int? remainingQuantity);

    void NotifyGroupCharacterInvite(int groupId, GroupInviteEntity groupInvite);
    void NotifyGroupCancelGroupInvite(int groupId, GroupInviteEntity groupInvite);
    void NotifyGroupAcceptGroupInvite(int groupId, GroupInviteEntity groupInvite);
    void NotifyGroupChangeGroupData(int groupId, GroupData groupData);
    void NotifyGroupChangeConfig(int groupId, GroupConfig groupConfig);
    void NotifyGroupAddLoot(int groupId, LootEntity loot);
    void NotifyGroupDeleteLoot(int groupId, int lootId);
    void NotifyGroupAddMonster(int groupId, MonsterEntity monster);
    void NotifyGroupKillMonster(MonsterEntity monster);

    void NotifyLootUpdateVisibility(int lootId, bool visibleForPlayer);
    void NotifyLootAddMonster(int lootId, MonsterEntity monster);
    void NotifyLootAddItem(int lootId, ItemEntity item);

    void NotifyMonsterAddModifier(int monsterId, ActiveStatsModifier modifier);
    void NotifyMonsterUpdateModifier(int monsterId, ActiveStatsModifier modifier);
    void NotifyMonsterRemoveModifier(int monsterId, int modifierId);
    void NotifyMonsterAddItem(int monsterId, ItemEntity item);
    void NotifyMonsterUpdateData(int monsterId, MonsterData monsterData);
    void NotifyMonsterChangeTarget(int monsterId, TargetRequest request);
    void NotifyMonsterChangeName(int monsterId, string name);

    Task CommitAsync();
}

public class NotificationSession(
    INotificationPacketBuilder packetBuilder,
    INotificationSender notificationSender
) : INotificationSession
{
    private readonly IList<INotificationPacket> _packets = new List<INotificationPacket>();

    public void NotifyCharacterChangeEv(CharacterEntity character)
    {
        _packets.Add(packetBuilder.BuildCharacterChangeEv(character));
    }

    public void NotifyCharacterChangeEa(CharacterEntity character)
    {
        _packets.Add(packetBuilder.BuildCharacterChangeEa(character));
    }

    public void NotifyCharacterChangeFatePoint(CharacterEntity character)
    {
        _packets.Add(packetBuilder.BuildCharacterChangeFatePoint(character));
    }

    public void NotifyCharacterChangeExperience(CharacterEntity character)
    {
        _packets.Add(packetBuilder.BuildCharacterChangeExperience(character));
    }

    public void NotifyCharacterChangeSex(CharacterEntity character)
    {
        _packets.Add(packetBuilder.BuildCharacterChangeSex(character));
    }

    public void NotifyCharacterChangeName(CharacterEntity character)
    {
        _packets.Add(packetBuilder.BuildCharacterChangeName(character));
    }

    public void NotifyCharacterChangeNotes(CharacterEntity character)
    {
        _packets.Add(packetBuilder.BuildCharacterChangeNotes(character));
    }

    public void NotifyCharacterAddItem(int characterId, ItemEntity item, bool delayBuildPacket = false)
    {
        if (delayBuildPacket)
            _packets.Add(new DelayedNotificationPacket(() => packetBuilder.BuildCharacterAddItem(characterId, item)));
        else
            _packets.Add(packetBuilder.BuildCharacterAddItem(characterId, item));
    }

    public void NotifyCharacterUpdateModifier(int characterId, CharacterModifierEntity modifier)
    {
        _packets.Add(packetBuilder.BuildCharacterUpdateModifier(characterId, modifier));
    }

    public void NotifyCharacterLevelUp(int characterId, LevelUpResult levelUpResult)
    {
        _packets.Add(packetBuilder.BuildCharacterLevelUp(characterId, levelUpResult));
    }

    public void NotifyCharacterAddJob(int characterId, Guid jobId)
    {
        _packets.Add(packetBuilder.BuildCharacterAddJob(characterId, jobId));
    }

    public void NotifyCharacterRemoveJob(int characterId, Guid jobId)
    {
        _packets.Add(packetBuilder.BuildCharacterRemoveJob(characterId, jobId));
    }

    public void NotifyCharacterGmChangeColor(CharacterEntity character)
    {
        _packets.Add(packetBuilder.BuildCharacterChangeColor(character));
    }

    public void NotifyCharacterGmChangeTarget(CharacterEntity character, TargetRequest targetInfo)
    {
        _packets.Add(packetBuilder.BuildCharacterChangeTarget(character, targetInfo));
    }

    public void NotifyCharacterGmChangeData(CharacterEntity character, CharacterGmData gmData)
    {
        _packets.Add(packetBuilder.BuildCharacterChangeData(character, gmData));
    }

    public void NotifyCharacterGmChangeActive(CharacterEntity character)
    {
        _packets.Add(packetBuilder.BuildCharacterChangeActive(character));
    }

    public void NotifyItemDataChanged(ItemEntity item)
    {
        _packets.Add(packetBuilder.BuildItemDataChanged(item));
    }

    public void NotifyItemModifiersChanged(ItemEntity item)
    {
        _packets.Add(packetBuilder.BuildItemModifiersChanged(item));
    }

    public void NotifyEquipItem(ItemEntity item)
    {
        _packets.Add(packetBuilder.BuildEquipItem(item));
    }

    public void NotifyItemChangeContainer(ItemEntity item)
    {
        _packets.Add(packetBuilder.BuildItemChangeContainer(item));
    }

    public void NotifyItemUpdateModifier(ItemEntity item)
    {
        _packets.Add(packetBuilder.BuildItemUpdateModifier(item));
    }

    public void NotifyItemDeleteItem(ItemEntity item)
    {
        _packets.Add(packetBuilder.BuildItemDeleteItem(item));
    }

    public void NotifyItemTakeItem(ItemEntity item, CharacterEntity character, int? remainingQuantity)
    {
        _packets.Add(packetBuilder.BuildItemTakeItem(item, character, remainingQuantity));
    }

    public void NotifyCharacterSetStatBonusAd(int characterId, string stat)
    {
        _packets.Add(packetBuilder.BuildCharacterSetStatBonusAd(characterId, stat));
    }

    public void NotifyCharacterAddModifier(int characterId, CharacterModifierEntity characterModifier, bool delayBuildPacket = false)
    {
        if (delayBuildPacket)
            _packets.Add(new DelayedNotificationPacket(() => packetBuilder.BuildCharacterAddModifier(characterId, characterModifier)));
        else
            _packets.Add(packetBuilder.BuildCharacterAddModifier(characterId, characterModifier));
    }

    public void NotifyCharacterRemoveModifier(int characterId, int characterModifierId)
    {
        _packets.Add(packetBuilder.BuildCharacterRemoveModifier(characterId, characterModifierId));
    }

    public void NotifyCharacterGroupInvite(int characterId, GroupInviteEntity groupInvite)
    {
        _packets.Add(packetBuilder.BuildCharacterGroupInvite(characterId, groupInvite));
    }

    public void NotifyCharacterCancelGroupInvite(int characterId, GroupInviteEntity groupInvite)
    {
        _packets.Add(packetBuilder.BuildCharacterCancelGroupInvite(characterId, groupInvite));
    }

    public void NotifyCharacterAcceptGroupInvite(int characterId, GroupInviteEntity groupInvite)
    {
        _packets.Add(packetBuilder.BuildCharacterAcceptGroupInvite(characterId, groupInvite));
    }

    public void NotifyCharacterShowLoot(int characterId, LootEntity loot)
    {
        _packets.Add(packetBuilder.BuildCharacterShowLoot(characterId, loot));
    }

    public void NotifyCharacterHideLoot(int characterId, int lootId)
    {
        _packets.Add(packetBuilder.BuildCharacterHideLoot(characterId, lootId));
    }

    public void NotifyGroupCharacterInvite(int groupId, GroupInviteEntity groupInvite)
    {
        _packets.Add(packetBuilder.BuildGroupCharacterInvite(groupId, groupInvite));
    }

    public void NotifyGroupCancelGroupInvite(int groupId, GroupInviteEntity groupInvite)
    {
        _packets.Add(packetBuilder.BuildGroupCancelGroupInvite(groupId, groupInvite));
    }

    public void NotifyGroupAcceptGroupInvite(int groupId, GroupInviteEntity groupInvite)
    {
        _packets.Add(packetBuilder.BuildGroupAcceptGroupInvite(groupId, groupInvite));
    }

    public void NotifyGroupChangeGroupData(int groupId, GroupData groupData)
    {
        _packets.Add(packetBuilder.BuildGroupChangeGroupData(groupId, groupData));
    }

    public void NotifyGroupChangeConfig(int groupId, GroupConfig groupConfig)
    {
        _packets.Add(packetBuilder.BuildGroupChangeConfig(groupId, groupConfig));
    }

    public void NotifyGroupAddLoot(int groupId, LootEntity loot)
    {
        _packets.Add(packetBuilder.BuildGroupAddLoot(groupId, loot));
    }

    public void NotifyGroupDeleteLoot(int groupId, int lootId)
    {
        _packets.Add(packetBuilder.BuildGroupDeleteLoot(groupId, lootId));
    }

    public void NotifyGroupAddMonster(int groupId, MonsterEntity monster)
    {
        _packets.Add(packetBuilder.BuildGroupAddMonster(groupId, monster));
    }

    public void NotifyGroupKillMonster(MonsterEntity monster)
    {
        _packets.Add(packetBuilder.BuildGroupKillMonster(monster.GroupId, monster.Id));
    }

    public void NotifyLootUpdateVisibility(int lootId, bool visibleForPlayer)
    {
        _packets.Add(packetBuilder.BuildLootUpdateVisibility(lootId, visibleForPlayer));
    }

    public void NotifyLootAddMonster(int lootId, MonsterEntity monster)
    {
        _packets.Add(packetBuilder.BuildLootAddMonster(lootId, monster));
    }

    public void NotifyLootAddItem(int lootId, ItemEntity item)
    {
        _packets.Add(packetBuilder.BuildLootAddItem(lootId, item));
    }

    public void NotifyMonsterAddModifier(int monsterId, ActiveStatsModifier modifier)
    {
        _packets.Add(packetBuilder.BuildMonsterAddModifier(monsterId, modifier));
    }

    public void NotifyMonsterUpdateModifier(int monsterId, ActiveStatsModifier modifier)
    {
        _packets.Add(packetBuilder.BuildMonsterUpdateModifier(monsterId, modifier));
    }

    public void NotifyMonsterRemoveModifier(int monsterId, int modifierId)
    {
        _packets.Add(packetBuilder.BuildMonsterRemoveModifier(monsterId, modifierId));
    }

    public void NotifyMonsterAddItem(int monsterId, ItemEntity item)
    {
        _packets.Add(packetBuilder.BuildMonsterAddItem(monsterId, item));
    }

    public void NotifyMonsterUpdateData(int monsterId, MonsterData monsterData)
    {
        _packets.Add(packetBuilder.BuildMonsterUpdateData(monsterId, monsterData));
    }

    public void NotifyMonsterChangeTarget(int monsterId, TargetRequest target)
    {
        _packets.Add(packetBuilder.BuildMonsterChangeTarget(monsterId, target));
    }

    public void NotifyMonsterChangeName(int monsterId, string name)
    {
        _packets.Add(packetBuilder.BuildMonsterChangeName(monsterId, name));
    }

    public async Task CommitAsync()
    {
        await notificationSender.SendPacketsAsync(_packets);
        _packets.Clear();
    }
}