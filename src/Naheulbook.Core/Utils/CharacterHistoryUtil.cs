using System;
using Naheulbook.Data.Models;
using Naheulbook.Shared.Utils;

namespace Naheulbook.Core.Utils;

public interface ICharacterHistoryUtil
{
    CharacterHistoryEntryEntity CreateLogChangeEv(CharacterEntity character, int? oldValue, int? newValue);
    CharacterHistoryEntryEntity CreateLogChangeEa(CharacterEntity character, int? oldValue, int? newValue);
    CharacterHistoryEntryEntity CreateLogChangeFatePoint(CharacterEntity character, short oldValue, short? newValue);
    CharacterHistoryEntryEntity CreateLogChangeExperience(CharacterEntity character, int oldValue, int? newValue);
    CharacterHistoryEntryEntity CreateLogChangeSex(CharacterEntity character, string oldValue, string newValue);
    CharacterHistoryEntryEntity CreateLogChangeName(CharacterEntity character, string oldValue, string newValue);
    CharacterHistoryEntryEntity CreateLogAddItem(int characterId, ItemEntity item);
    CharacterHistoryEntryEntity CreateLogEquipItem(int characterId, int itemId);
    CharacterHistoryEntryEntity CreateLogUnEquipItem(int characterId, int itemId);
    CharacterHistoryEntryEntity CreateLogAddModifier(CharacterEntity character, CharacterModifierEntity characterModifier);
    CharacterHistoryEntryEntity CreateLogRemoveModifier(int characterId, int characterModifierId);
    CharacterHistoryEntryEntity CreateLogActiveModifier(int characterId, int characterModifierId);
    CharacterHistoryEntryEntity CreateLogDisableModifier(int characterId, int characterModifierId);
    CharacterHistoryEntryEntity CreateLogGiveItem(int characterId, ItemEntity item);
    CharacterHistoryEntryEntity CreateLogGivenItem(int characterId, ItemEntity item);
    CharacterHistoryEntryEntity CreateLogLootItem(int characterId, ItemEntity item);
    CharacterHistoryEntryEntity CreateLogChangeItemQuantity(int characterId, ItemEntity item, int? oldValue, int? newValue);
    CharacterHistoryEntryEntity CreateLogUseItemCharge(int characterId, ItemEntity item, int? oldValue, int? newValue);
    CharacterHistoryEntryEntity CreateLogReadBook(int characterId, ItemEntity item);
    CharacterHistoryEntryEntity CreateLogIdentifyItem(int characterId, ItemEntity item);
    CharacterHistoryEntryEntity CreateLogLevelUp(int characterId, int characterLevel);
}

public class CharacterHistoryUtil : ICharacterHistoryUtil
{
    private const string AddItemActionName = "ADD_ITEM";
    private const string ChangeEvActionName = "MODIFY_EV";
    private const string ChangeEaActionName = "MODIFY_EA";
    private const string ChangeFatePointActionName = "USE_FATE_POINT";
    private const string ChangeExperienceActionName = "ADD_XP";
    private const string ChangeSexActionName = "CHANGE_SEX";
    private const string ChangeNameActionName = "CHANGE_NAME";
    private const string EquipActionName = "EQUIP";
    private const string UnEquipActionName = "UNEQUIP";
    private const string ApplyModifierActionName = "APPLY_MODIFIER";
    private const string RemoveModifierActionName = "REMOVE_MODIFIER";
    private const string ActiveModifierActionName = "ACTIVE_MODIFIER";
    private const string DisableModifierActionName = "DISABLE_MODIFIER";
    private const string GiveItemActionName = "GIVE_ITEM";
    private const string GivenItemActionName = "GIVEN_ITEM";
    private const string LootItemActionName = "LOOT_ITEM";
    private const string ChangeQuantityActionName = "CHANGE_QUANTITY";
    private const string UseChargeActionName = "USE_CHARGE";
    private const string ReadBookActionName = "READ_BOOK";
    private const string IdentifyActionName = "IDENTIFY";
    private const string LevelUpActionName = "LEVEL_UP";

    private readonly IJsonUtil _jsonUtil;

    public CharacterHistoryUtil(IJsonUtil jsonUtil)
    {
        _jsonUtil = jsonUtil;
    }

    public CharacterHistoryEntryEntity CreateLogChangeEv(CharacterEntity character, int? oldValue, int? newValue)
    {
        return new CharacterHistoryEntryEntity
        {
            Character = character,
            Action = ChangeEvActionName,
            Date = DateTime.Now,
            Data = _jsonUtil.Serialize(new {oldValue, newValue}),
        };
    }

    public CharacterHistoryEntryEntity CreateLogChangeEa(CharacterEntity character, int? oldValue, int? newValue)
    {
        return new CharacterHistoryEntryEntity
        {
            Character = character,
            Action = ChangeEaActionName,
            Date = DateTime.Now,
            Data = _jsonUtil.Serialize(new {oldValue, newValue}),
        };
    }

    public CharacterHistoryEntryEntity CreateLogChangeFatePoint(CharacterEntity character, short oldValue, short? newValue)
    {
        return new CharacterHistoryEntryEntity
        {
            Character = character,
            Action = ChangeFatePointActionName,
            Date = DateTime.Now,
            Data = _jsonUtil.Serialize(new {oldValue, newValue}),
        };
    }

    public CharacterHistoryEntryEntity CreateLogChangeExperience(CharacterEntity character, int oldValue, int? newValue)
    {
        return new CharacterHistoryEntryEntity
        {
            Character = character,
            Action = ChangeExperienceActionName,
            Date = DateTime.Now,
            Data = _jsonUtil.Serialize(new {oldValue, newValue}),
        };
    }

    public CharacterHistoryEntryEntity CreateLogChangeSex(CharacterEntity character, string oldValue, string newValue)
    {
        return new CharacterHistoryEntryEntity
        {
            Character = character,
            Action = ChangeSexActionName,
            Date = DateTime.Now,
            Data = _jsonUtil.Serialize(new {oldValue, newValue}),
        };
    }

    public CharacterHistoryEntryEntity CreateLogChangeName(CharacterEntity character, string oldValue, string newValue)
    {
        return new CharacterHistoryEntryEntity
        {
            Character = character,
            Action = ChangeNameActionName,
            Date = DateTime.Now,
            Data = _jsonUtil.Serialize(new {oldValue, newValue}),
        };
    }

    public CharacterHistoryEntryEntity CreateLogAddItem(int characterId, ItemEntity item)
    {
        return new CharacterHistoryEntryEntity
        {
            CharacterId = characterId,
            Action = AddItemActionName,
            Date = DateTime.Now,
            ItemId = item.Id,
        };
    }

    public CharacterHistoryEntryEntity CreateLogEquipItem(int characterId, int itemId)
    {
        return new CharacterHistoryEntryEntity
        {
            CharacterId = characterId,
            Action = EquipActionName,
            Date = DateTime.Now,
            ItemId = itemId,
        };
    }

    public CharacterHistoryEntryEntity CreateLogUnEquipItem(int characterId, int itemId)
    {
        return new CharacterHistoryEntryEntity
        {
            CharacterId = characterId,
            Action = UnEquipActionName,
            Date = DateTime.Now,
            ItemId = itemId,
        };
    }

    public CharacterHistoryEntryEntity CreateLogAddModifier(CharacterEntity character, CharacterModifierEntity characterModifier)
    {
        return new CharacterHistoryEntryEntity
        {
            CharacterId = character.Id,
            Action = ApplyModifierActionName,
            Date = DateTime.Now,
            CharacterModifier = characterModifier,
        };
    }

    public CharacterHistoryEntryEntity CreateLogRemoveModifier(int characterId, int characterModifierId)
    {
        return new CharacterHistoryEntryEntity
        {
            CharacterId = characterId,
            Action = RemoveModifierActionName,
            Date = DateTime.Now,
            CharacterModifierId = characterModifierId,
        };
    }

    public CharacterHistoryEntryEntity CreateLogActiveModifier(int characterId, int characterModifierId)
    {
        return new CharacterHistoryEntryEntity
        {
            CharacterId = characterId,
            Action = ActiveModifierActionName,
            Date = DateTime.Now,
            CharacterModifierId = characterModifierId,
        };
    }

    public CharacterHistoryEntryEntity CreateLogDisableModifier(int characterId, int characterModifierId)
    {
        return new CharacterHistoryEntryEntity
        {
            CharacterId = characterId,
            Action = DisableModifierActionName,
            Date = DateTime.Now,
            CharacterModifierId = characterModifierId,
        };
    }

    public CharacterHistoryEntryEntity CreateLogGiveItem(int characterId, ItemEntity item)
    {
        return new CharacterHistoryEntryEntity
        {
            CharacterId = characterId,
            Action = GiveItemActionName,
            Date = DateTime.Now,
            ItemId = item.Id,
        };
    }

    public CharacterHistoryEntryEntity CreateLogGivenItem(int characterId, ItemEntity item)
    {
        return new CharacterHistoryEntryEntity
        {
            CharacterId = characterId,
            Action = GivenItemActionName,
            Date = DateTime.Now,
            Item = item,
        };
    }

    public CharacterHistoryEntryEntity CreateLogLootItem(int characterId, ItemEntity item)
    {
        return new CharacterHistoryEntryEntity
        {
            CharacterId = characterId,
            Action = LootItemActionName,
            Date = DateTime.Now,
            Item = item,
        };
    }

    public CharacterHistoryEntryEntity CreateLogChangeItemQuantity(int characterId, ItemEntity item, int? oldValue, int? newValue)
    {
        return new CharacterHistoryEntryEntity
        {
            CharacterId = characterId,
            Action = ChangeQuantityActionName,
            Date = DateTime.Now,
            ItemId = item.Id,
            Data = _jsonUtil.Serialize(new {oldValue, newValue}),
        };
    }

    public CharacterHistoryEntryEntity CreateLogUseItemCharge(int characterId, ItemEntity item, int? oldValue, int? newValue)
    {
        return new CharacterHistoryEntryEntity
        {
            CharacterId = characterId,
            Action = UseChargeActionName,
            Date = DateTime.Now,
            ItemId = item.Id,
            Data = _jsonUtil.Serialize(new {oldValue, newValue}),
        };
    }

    public CharacterHistoryEntryEntity CreateLogReadBook(int characterId, ItemEntity item)
    {
        return new CharacterHistoryEntryEntity
        {
            CharacterId = characterId,
            Action = ReadBookActionName,
            Date = DateTime.Now,
            ItemId = item.Id,
        };
    }

    public CharacterHistoryEntryEntity CreateLogIdentifyItem(int characterId, ItemEntity item)
    {
        return new CharacterHistoryEntryEntity
        {
            CharacterId = characterId,
            Action = IdentifyActionName,
            Date = DateTime.Now,
            ItemId = item.Id,
        };
    }

    public CharacterHistoryEntryEntity CreateLogLevelUp(int characterId, int level)
    {
        return new CharacterHistoryEntryEntity
        {
            CharacterId = characterId,
            Action = LevelUpActionName,
            Date = DateTime.Now,
            Info = level.ToString(),
        };
    }
}