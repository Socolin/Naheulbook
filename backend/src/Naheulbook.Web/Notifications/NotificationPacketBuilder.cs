using AutoMapper;
using Naheulbook.Core.Features.Character;
using Naheulbook.Core.Features.Group;
using Naheulbook.Core.Notifications;
using Naheulbook.Data.EntityFrameworkCore.Entities;
using Naheulbook.Requests.Requests;
using Naheulbook.Shared.TransientModels;
using Naheulbook.Web.Responses;
using Naheulbook.Web.Services;

namespace Naheulbook.Web.Notifications;

public class NotificationPacketBuilder(IMapper mapper, IHubGroupUtil hubGroupUtil) : INotificationPacketBuilder
{
    private enum ElementType
    {
        Character,
        Group,
        Monster,
        Loot,
        Fight,
    }

    public INotificationPacket BuildCharacterChangeEv(CharacterEntity character)
    {
        return BuildCharacterChange(character.Id, "update", new {Stat = "ev", Value = character.Ev});
    }

    public INotificationPacket BuildCharacterChangeEa(CharacterEntity character)
    {
        return BuildCharacterChange(character.Id, "update", new {Stat = "ea", Value = character.Ea});
    }

    public INotificationPacket BuildCharacterChangeFatePoint(CharacterEntity character)
    {
        return BuildCharacterChange(character.Id, "update", new {Stat = "fatePoint", Value = character.FatePoint});
    }

    public INotificationPacket BuildCharacterChangeExperience(CharacterEntity character)
    {
        return BuildCharacterChange(character.Id, "update", new {Stat = "experience", Value = character.Experience});
    }

    public INotificationPacket BuildCharacterChangeSex(CharacterEntity character)
    {
        return BuildCharacterChange(character.Id, "update", new {Stat = "sex", Value = character.Sex});
    }

    public INotificationPacket BuildCharacterChangeName(CharacterEntity character)
    {
        return BuildCharacterChange(character.Id, "update", new {Stat = "name", Value = character.Name});
    }

    public INotificationPacket BuildCharacterChangeNotes(CharacterEntity character)
    {
        return BuildCharacterChange(character.Id, "update", new {Stat = "notes", Value = character.Notes});
    }

    public INotificationPacket BuildCharacterAddItem(int characterId, ItemEntity item)
    {
        return BuildCharacterChange(characterId, "addItem", mapper.Map<ItemResponse>(item));
    }

    public INotificationPacket BuildCharacterLevelUp(int characterId, LevelUpResult levelUpResult)
    {
        return BuildCharacterChange(characterId, "levelUp", mapper.Map<CharacterLevelUpResponse>(levelUpResult));
    }

    public INotificationPacket BuildCharacterAddJob(int characterId, Guid jobId)
    {
        return BuildCharacterChange(characterId, "addJob", new CharacterAddJobResponse {JobId = jobId});
    }

    public INotificationPacket BuildCharacterRemoveJob(int characterId, Guid jobId)
    {
        return BuildCharacterChange(characterId, "removeJob", new CharacterRemoveJobResponse {JobId = jobId});
    }

    public INotificationPacket BuildCharacterAddAptitude(int characterId, CharacterAptitudeEntity characterAptitude)
    {
        return BuildCharacterChange(characterId, "addAptitude", mapper.Map<CharacterAddAptitudeResponse>(characterAptitude));
    }

    public INotificationPacket BuildCharacterRemoveAptitude(int characterId, CharacterAptitudeEntity characterAptitude)
    {
        return BuildCharacterChange(characterId, "removeAptitude", mapper.Map<CharacterAptitudeResponse>(characterAptitude));
    }

    public INotificationPacket BuildCharacterUpdateAptitude(int characterId, CharacterAptitudeEntity characterAptitude)
    {
        return BuildCharacterChange(characterId, "updateAptitude", mapper.Map<CharacterAptitudeResponse>(characterAptitude));
    }

    public INotificationPacket BuildCharacterChangeColor(CharacterEntity character)
    {
        return BuildCharacterGmChange(character.Id, "changeColor", character.Color);
    }

    public INotificationPacket BuildCharacterChangeTarget(CharacterEntity character, TargetRequest targetInfo)
    {
        return BuildCharacterGmChange(character.Id, "changeTarget", targetInfo);
    }

    public INotificationPacket BuildCharacterChangeData(CharacterEntity character, CharacterGmData gmData)
    {
        return BuildCharacterGmChange(character.Id, "updateGmData", gmData);
    }

    public INotificationPacket BuildCharacterChangeActive(CharacterEntity character)
    {
        return BuildCharacterGmChange(character.Id, "active", character.IsActive);
    }

    public INotificationPacket BuildItemDataChanged(ItemEntity item)
    {
        if (item.CharacterId != null)
            return BuildCharacterChange(item.CharacterId.Value, "updateItem", mapper.Map<ItemPartialResponse>(item));
        if (item.MonsterId != null)
            return BuildMonsterChange(item.MonsterId.Value, "updateItem", mapper.Map<ItemPartialResponse>(item));
        if (item.LootId != null)
            return BuildLootChange(item.LootId.Value, "updateItem", mapper.Map<ItemPartialResponse>(item));

        throw new NotSupportedException();
    }

    public INotificationPacket BuildItemModifiersChanged(ItemEntity item)
    {
        if (item.CharacterId != null)
            return BuildCharacterChange(item.CharacterId.Value, "updateItemModifiers", mapper.Map<ItemPartialResponse>(item));
        if (item.MonsterId != null)
            return BuildMonsterChange(item.MonsterId.Value, "updateItemModifiers", mapper.Map<ItemPartialResponse>(item));
        if (item.LootId != null)
            return BuildLootChange(item.LootId.Value, "updateItemModifiers", mapper.Map<ItemPartialResponse>(item));

        throw new NotSupportedException();
    }

    public INotificationPacket BuildEquipItem(ItemEntity item)
    {
        if (item.CharacterId != null)
            return BuildCharacterChange(item.CharacterId.Value, "equipItem", mapper.Map<ItemPartialResponse>(item));
        if (item.MonsterId != null)
            return BuildMonsterChange(item.MonsterId.Value, "equipItem", mapper.Map<ItemPartialResponse>(item));
        if (item.LootId != null)
            return BuildLootChange(item.LootId.Value, "equipItem", mapper.Map<ItemPartialResponse>(item));

        throw new NotSupportedException();
    }

    public INotificationPacket BuildItemChangeContainer(ItemEntity item)
    {
        if (item.CharacterId != null)
            return BuildCharacterChange(item.CharacterId.Value, "changeContainer", mapper.Map<ItemPartialResponse>(item));
        if (item.MonsterId != null)
            return BuildMonsterChange(item.MonsterId.Value, "changeContainer", mapper.Map<ItemPartialResponse>(item));
        if (item.LootId != null)
            return BuildLootChange(item.LootId.Value, "changeContainer", mapper.Map<ItemPartialResponse>(item));

        throw new NotSupportedException();
    }

    public INotificationPacket BuildItemUpdateModifier(ItemEntity item)
    {
        if (item.CharacterId != null)
            return BuildCharacterChange(item.CharacterId.Value, "updateItemModifiers", mapper.Map<ItemPartialResponse>(item));
        if (item.MonsterId != null)
            return BuildMonsterChange(item.MonsterId.Value, "updateItemModifiers", mapper.Map<ItemPartialResponse>(item));
        if (item.LootId != null)
            return BuildLootChange(item.LootId.Value, "updateItemModifiers", mapper.Map<ItemPartialResponse>(item));

        throw new NotSupportedException();
    }

    public INotificationPacket BuildItemDeleteItem(ItemEntity item)
    {
        if (item.CharacterId != null)
            return BuildCharacterChange(item.CharacterId.Value, "deleteItem", item.Id);
        if (item.MonsterId != null)
            return BuildMonsterChange(item.MonsterId.Value, "deleteItem", item.Id);
        if (item.LootId != null)
            return BuildLootChange(item.LootId.Value, "deleteItem", item.Id);

        throw new NotSupportedException();
    }

    public INotificationPacket BuildItemTakeItem(ItemEntity item, CharacterEntity character, int? remainingQuantity)
    {
        if (item.CharacterId != null)
            return BuildCharacterChange(item.CharacterId.Value, "tookItem", new {Character = mapper.Map<NamedIdResponse>(character), remainingQuantity, OriginalItem = mapper.Map<ItemPartialResponse>(item)});
        if (item.MonsterId != null)
            return BuildMonsterChange(item.MonsterId.Value, "tookItem", new {Character = mapper.Map<NamedIdResponse>(character), remainingQuantity, OriginalItem = mapper.Map<ItemPartialResponse>(item)});
        if (item.LootId != null)
            return BuildLootChange(item.LootId.Value, "tookItem", new {Character = mapper.Map<NamedIdResponse>(character), remainingQuantity, OriginalItem = mapper.Map<ItemPartialResponse>(item)});

        throw new NotSupportedException();
    }

    public INotificationPacket BuildCharacterSetStatBonusAd(int characterId, string stat)
    {
        return BuildCharacterChange(characterId, "statBonusAd", stat);
    }

    public INotificationPacket BuildCharacterAddModifier(int characterId, CharacterModifierEntity characterModifier)
    {
        return BuildCharacterChange(characterId, "addModifier", mapper.Map<ActiveStatsModifier>(characterModifier));
    }

    public INotificationPacket BuildCharacterRemoveModifier(int characterId, int characterModifierId)
    {
        return BuildCharacterChange(characterId, "removeModifier", characterModifierId);
    }

    public INotificationPacket BuildCharacterUpdateModifier(int characterId, CharacterModifierEntity characterModifier)
    {
        return BuildCharacterChange(characterId, "updateModifier", mapper.Map<ActiveStatsModifier>(characterModifier));
    }

    public INotificationPacket BuildCharacterGroupInvite(int characterId, GroupInviteEntity groupInvite)
    {
        return BuildCharacterChange(characterId, "groupInvite", mapper.Map<CharacterGroupInviteResponse>(groupInvite));
    }

    public INotificationPacket BuildCharacterCancelGroupInvite(int characterId, GroupInviteEntity groupInvite)
    {
        return BuildCharacterChange(characterId, "cancelInvite", mapper.Map<DeleteInviteResponse>(groupInvite));
    }

    public INotificationPacket BuildCharacterAcceptGroupInvite(int characterId, GroupInviteEntity groupInvite)
    {
        return BuildCharacterChange(characterId, "joinGroup", mapper.Map<CharacterGroupResponse>(groupInvite.Group));
    }

    public INotificationPacket BuildCharacterShowLoot(int characterId, LootEntity loot)
    {
        return BuildCharacterChange(characterId, "showLoot", mapper.Map<LootResponse>(loot));
    }

    public INotificationPacket BuildCharacterHideLoot(int characterId, int lootId)
    {
        return BuildCharacterChange(characterId, "hideLoot", lootId);
    }

    public INotificationPacket BuildGroupCharacterInvite(int groupId, GroupInviteEntity groupInvite)
    {
        return BuildGroupChange(groupId, "groupInvite", mapper.Map<GroupGroupInviteResponse>(groupInvite));
    }

    public INotificationPacket BuildGroupCancelGroupInvite(int groupId, GroupInviteEntity groupInvite)
    {
        return BuildGroupChange(groupId, "cancelInvite", mapper.Map<DeleteInviteResponse>(groupInvite));
    }

    public INotificationPacket BuildGroupAcceptGroupInvite(int groupId, GroupInviteEntity groupInvite)
    {
        return BuildGroupChange(groupId, "joinCharacter", groupInvite.CharacterId);
    }

    public INotificationPacket BuildGroupChangeGroupData(int groupId, GroupData groupData)
    {
        return BuildGroupChange(groupId, "changeData", groupData);
    }

    public INotificationPacket BuildGroupAddLoot(int groupId, LootEntity loot)
    {
        return BuildGroupChange(groupId, "addLoot", mapper.Map<LootResponse>(loot));
    }

    public INotificationPacket BuildGroupDeleteLoot(int groupId, int lootId)
    {
        return BuildGroupChange(groupId, "deleteLoot", lootId);
    }

    public INotificationPacket BuildGroupAddFight(int groupId, FightEntity fight)
    {
        return BuildGroupChange(groupId, "addFight", mapper.Map<FightResponse>(fight));
    }

    public INotificationPacket BuildGroupDeleteFight(int groupId, int fightId)
    {
        return BuildGroupChange(groupId, "deleteFight", fightId);
    }

    public INotificationPacket BuildGroupChangeConfig(in int groupId, GroupConfig groupConfig)
    {
        return BuildGroupChange(groupId, "changeConfig", groupConfig);
    }

    public INotificationPacket BuildGroupAddMerchant(int groupId, MerchantEntity merchant)
    {
        return BuildGroupChange(groupId, "addMerchant", mapper.Map<MerchantResponse>(merchant));
    }

    public INotificationPacket BuildGroupAddMonster(int groupId, MonsterEntity monster)
    {
        return BuildGroupChange(groupId, "addMonster", mapper.Map<MonsterResponse>(monster));
    }

    public INotificationPacket BuildGroupKillMonster(int groupId, int monsterId)
    {
        return BuildGroupChange(groupId, "killMonster", monsterId);
    }

    public INotificationPacket BuildGroupDeleteMonster(int groupId, int monsterId)
    {
        return BuildGroupChange(groupId, "deleteMonster", monsterId);
    }

    public INotificationPacket BuildLootUpdateVisibility(int lootId, bool visibleForPlayer)
    {
        return BuildLootChange(lootId, "updateVisibility", visibleForPlayer);
    }

    public INotificationPacket BuildLootAddMonster(int lootId, MonsterEntity monster)
    {
        return BuildLootChange(lootId, "addMonster", mapper.Map<MonsterResponse>(monster));
    }

    public INotificationPacket BuildLootAddItem(int lootId, ItemEntity item)
    {
        return BuildLootChange(lootId, "addItem", mapper.Map<ItemResponse>(item));
    }

    public INotificationPacket BuildLootDeleteMonster(int lootId, int monsterId)
    {
        return BuildLootChange(lootId, "deleteMonster", monsterId);
    }

    public INotificationPacket BuildMonsterAddModifier(int monsterId, ActiveStatsModifier modifier)
    {
        return BuildMonsterChange(monsterId, "addModifier", modifier);
    }

    public INotificationPacket BuildMonsterUpdateModifier(int monsterId, ActiveStatsModifier modifier)
    {
        return BuildMonsterChange(monsterId, "updateModifier", modifier);
    }

    public INotificationPacket BuildMonsterRemoveModifier(int monsterId, int modifierId)
    {
        return BuildMonsterChange(monsterId, "removeModifier", modifierId);
    }

    public INotificationPacket BuildMonsterAddItem(int monsterId, ItemEntity item)
    {
        return BuildMonsterChange(monsterId, "addItem", mapper.Map<ItemResponse>(item));
    }

    public INotificationPacket BuildMonsterUpdateData(int monsterId, MonsterData monsterData)
    {
        return BuildMonsterChange(monsterId, "changeData", monsterData);
    }

    public INotificationPacket BuildMonsterChangeTarget(int monsterId, TargetRequest targetInfo)
    {
        return BuildMonsterChange(monsterId, "changeTarget", targetInfo);
    }

    public INotificationPacket BuildMonsterChangeName(int monsterId, string name)
    {
        return BuildMonsterChange(monsterId, "changeName", name);
    }

    public INotificationPacket BuildFightAddMonster(int fightId, MonsterEntity monster)
    {
        return BuildFightChange(fightId, "addMonster", mapper.Map<MonsterResponse>(monster));
    }

    public INotificationPacket BuildFightRemoveMonster(int fightId, int monsterId)
    {
        return BuildFightChange(fightId, "removeMonster", monsterId);
    }

    private INotificationPacket BuildCharacterChange(int characterId, string action, object data)
    {
        return new NotificationPacket(
            hubGroupUtil.GetCharacterGroupName(characterId),
            GetPacket(ElementType.Character, characterId, action, data)
        );
    }

    private INotificationPacket BuildCharacterGmChange(int characterId, string action, object data)
    {
        return new NotificationPacket(
            hubGroupUtil.GetGmCharacterGroupName(characterId),
            GetPacket(ElementType.Character, characterId, action, data)
        );
    }

    private INotificationPacket BuildGroupChange(int groupId, string action, object data)
    {
        return new NotificationPacket(
            hubGroupUtil.GetGroupGroupName(groupId),
            GetPacket(ElementType.Group, groupId, action, data)
        );
    }

    private INotificationPacket BuildLootChange(int lootId, string action, object data)
    {
        return new NotificationPacket(
            hubGroupUtil.GetLootGroupName(lootId),
            GetPacket(ElementType.Loot, lootId, action, data)
        );
    }

    private INotificationPacket BuildMonsterChange(int monsterId, string action, object data)
    {
        return new NotificationPacket(
            hubGroupUtil.GetMonsterGroupName(monsterId),
            GetPacket(ElementType.Monster, monsterId, action, data)
        );
    }

    private INotificationPacket BuildFightChange(int fightId, string action, object data)
    {
        return new NotificationPacket(
            hubGroupUtil.GetFightGroupName(fightId),
            GetPacket(ElementType.Fight, fightId, action, data)
        );
    }

    private static NotificationPacketPayload GetPacket(ElementType elementType, int elementId, string opcode, object data)
    {
        return new NotificationPacketPayload(
            elementId,
            elementType.ToString().ToLowerInvariant(),
            opcode,
            data
        );
    }
}