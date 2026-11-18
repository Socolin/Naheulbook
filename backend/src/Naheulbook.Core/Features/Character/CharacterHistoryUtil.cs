using Naheulbook.Core.Features.Item;
using Naheulbook.Data.EntityFrameworkCore.Entities;
using Naheulbook.Shared.Utils;

namespace Naheulbook.Core.Features.Character;

public interface ICharacterHistoryUtil
{
    CharacterHistoryEntryEntity CreateLogChangeEv(CharacterEntity character, int? oldValue, int? newValue);
    CharacterHistoryEntryEntity CreateLogChangeEa(CharacterEntity character, int? oldValue, int? newValue);
    CharacterHistoryEntryEntity CreateLogChangeFatePoint(CharacterEntity character, short oldValue, short? newValue);
    CharacterHistoryEntryEntity CreateLogChangeExperience(CharacterEntity character, int oldValue, int? newValue);
    CharacterHistoryEntryEntity CreateLogChangeSex(CharacterEntity character, string oldValue, string newValue);
    CharacterHistoryEntryEntity CreateLogChangeName(CharacterEntity character, string oldValue, string newValue);
    CharacterHistoryEntryEntity CreateLogAddItem(int characterId, ItemEntity item);
    CharacterHistoryEntryEntity CreateLogEquipItem(int characterId, ItemEntity item);
    CharacterHistoryEntryEntity CreateLogUnEquipItem(int characterId, ItemEntity item);
    CharacterHistoryEntryEntity CreateLogAddModifier(CharacterEntity character, CharacterModifierEntity characterModifier);
    CharacterHistoryEntryEntity CreateLogRemoveModifier(int characterId, CharacterModifierEntity characterModifier);
    CharacterHistoryEntryEntity CreateLogActiveModifier(int characterId, CharacterModifierEntity characterModifier);
    CharacterHistoryEntryEntity CreateLogDisableModifier(int characterId, CharacterModifierEntity characterModifier);
    CharacterHistoryEntryEntity CreateLogGiveItem(int characterId, ItemEntity item, string? targetCharacterName);
    CharacterHistoryEntryEntity CreateLogGivenItem(int characterId, ItemEntity item, string? sourceCharacterName);
    CharacterHistoryEntryEntity CreateLogLootItem(int characterId, ItemEntity item);
    CharacterHistoryEntryEntity CreateLogChangeItemQuantity(int characterId, ItemEntity item, int? oldValue, int? newValue);
    CharacterHistoryEntryEntity CreateLogUseItemCharge(int characterId, ItemEntity item, int? oldValue, int? newValue);
    CharacterHistoryEntryEntity CreateLogReadBook(int characterId, ItemEntity item);
    CharacterHistoryEntryEntity CreateLogIdentifyItem(int characterId, ItemEntity item);
    CharacterHistoryEntryEntity CreateLogLevelUp(int characterId, int characterLevel);
    CharacterHistoryEntryEntity CreateAddAptitude(int characterId, AptitudeEntity aptitude);
    CharacterHistoryEntryEntity CreateRemoveAptitude(int characterId, AptitudeEntity aptitude);
}

public class CharacterHistoryUtil(
    IJsonUtil jsonUtil,
    IItemDataUtil itemDataUtil
) : ICharacterHistoryUtil
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
    private const string AddAptitudeActionName = "ADD_APTITUDE";
    private const string RemoveAptitudeActionName = "REMOVE_APTITUDE";

    public CharacterHistoryEntryEntity CreateLogChangeEv(CharacterEntity character, int? oldValue, int? newValue)
    {
        return new CharacterHistoryEntryEntity
        {
            Character = character,
            Action = ChangeEvActionName,
            Date = DateTime.Now,
            Data = jsonUtil.Serialize(new {oldValue, newValue}),
        };
    }

    public CharacterHistoryEntryEntity CreateLogChangeEa(CharacterEntity character, int? oldValue, int? newValue)
    {
        return new CharacterHistoryEntryEntity
        {
            Character = character,
            Action = ChangeEaActionName,
            Date = DateTime.Now,
            Data = jsonUtil.Serialize(new {oldValue, newValue}),
        };
    }

    public CharacterHistoryEntryEntity CreateLogChangeFatePoint(CharacterEntity character, short oldValue, short? newValue)
    {
        return new CharacterHistoryEntryEntity
        {
            Character = character,
            Action = ChangeFatePointActionName,
            Date = DateTime.Now,
            Data = jsonUtil.Serialize(new {oldValue, newValue}),
        };
    }

    public CharacterHistoryEntryEntity CreateLogChangeExperience(CharacterEntity character, int oldValue, int? newValue)
    {
        return new CharacterHistoryEntryEntity
        {
            Character = character,
            Action = ChangeExperienceActionName,
            Date = DateTime.Now,
            Data = jsonUtil.Serialize(new {oldValue, newValue}),
        };
    }

    public CharacterHistoryEntryEntity CreateLogChangeSex(CharacterEntity character, string oldValue, string newValue)
    {
        return new CharacterHistoryEntryEntity
        {
            Character = character,
            Action = ChangeSexActionName,
            Date = DateTime.Now,
            Data = jsonUtil.Serialize(new {oldValue, newValue}),
        };
    }

    public CharacterHistoryEntryEntity CreateLogChangeName(CharacterEntity character, string oldValue, string newValue)
    {
        return new CharacterHistoryEntryEntity
        {
            Character = character,
            Action = ChangeNameActionName,
            Date = DateTime.Now,
            Data = jsonUtil.Serialize(new {oldValue, newValue}),
        };
    }

    public CharacterHistoryEntryEntity CreateLogAddItem(int characterId, ItemEntity item)
    {
        return new CharacterHistoryEntryEntity
        {
            CharacterId = characterId,
            Action = AddItemActionName,
            Date = DateTime.Now,
            Data = jsonUtil.Serialize(new {item = GetItemLogData(item)}),
        };
    }

    public CharacterHistoryEntryEntity CreateLogEquipItem(int characterId, ItemEntity item)
    {
        return new CharacterHistoryEntryEntity
        {
            CharacterId = characterId,
            Action = EquipActionName,
            Date = DateTime.Now,
            Data = jsonUtil.Serialize(new {item = GetItemLogData(item)}),
        };
    }

    public CharacterHistoryEntryEntity CreateLogUnEquipItem(int characterId, ItemEntity item)
    {
        return new CharacterHistoryEntryEntity
        {
            CharacterId = characterId,
            Action = UnEquipActionName,
            Date = DateTime.Now,
            Data = jsonUtil.Serialize(new {item = GetItemLogData(item)}),
        };
    }

    public CharacterHistoryEntryEntity CreateLogAddModifier(CharacterEntity character, CharacterModifierEntity characterModifier)
    {
        return new CharacterHistoryEntryEntity
        {
            CharacterId = character.Id,
            Action = ApplyModifierActionName,
            Date = DateTime.Now,
            Data = jsonUtil.Serialize(new {modifier = GetModifierLogData(characterModifier)}),
        };
    }

    public CharacterHistoryEntryEntity CreateLogRemoveModifier(int characterId, CharacterModifierEntity characterModifier)
    {
        return new CharacterHistoryEntryEntity
        {
            CharacterId = characterId,
            Action = RemoveModifierActionName,
            Date = DateTime.Now,
            Data = jsonUtil.Serialize(new {modifier = GetModifierLogData(characterModifier)}),
        };
    }

    public CharacterHistoryEntryEntity CreateLogActiveModifier(int characterId, CharacterModifierEntity characterModifier)
    {
        return new CharacterHistoryEntryEntity
        {
            CharacterId = characterId,
            Action = ActiveModifierActionName,
            Date = DateTime.Now,
            Data = jsonUtil.Serialize(new {modifier = GetModifierLogData(characterModifier)}),
        };
    }

    public CharacterHistoryEntryEntity CreateLogDisableModifier(int characterId, CharacterModifierEntity characterModifier)
    {
        return new CharacterHistoryEntryEntity
        {
            CharacterId = characterId,
            Action = DisableModifierActionName,
            Date = DateTime.Now,
            Data = jsonUtil.Serialize(new {modifier = GetModifierLogData(characterModifier)}),
        };
    }

    public CharacterHistoryEntryEntity CreateLogGiveItem(int characterId, ItemEntity item, string? targetCharacterName)
    {
        return new CharacterHistoryEntryEntity
        {
            CharacterId = characterId,
            Action = GiveItemActionName,
            Date = DateTime.Now,
            Data = jsonUtil.Serialize(new {item = GetItemLogData(item), characterName = targetCharacterName}),
        };
    }

    public CharacterHistoryEntryEntity CreateLogGivenItem(int characterId, ItemEntity item, string? sourceCharacterName)
    {
        return new CharacterHistoryEntryEntity
        {
            CharacterId = characterId,
            Action = GivenItemActionName,
            Date = DateTime.Now,
            Data = jsonUtil.Serialize(new {item = GetItemLogData(item), characterName = sourceCharacterName}),
        };
    }

    public CharacterHistoryEntryEntity CreateLogLootItem(int characterId, ItemEntity item)
    {
        return new CharacterHistoryEntryEntity
        {
            CharacterId = characterId,
            Action = LootItemActionName,
            Date = DateTime.Now,
            Data = jsonUtil.Serialize(new {item = GetItemLogData(item)}),
        };
    }

    public CharacterHistoryEntryEntity CreateLogChangeItemQuantity(int characterId, ItemEntity item, int? oldValue, int? newValue)
    {
        return new CharacterHistoryEntryEntity
        {
            CharacterId = characterId,
            Action = ChangeQuantityActionName,
            Date = DateTime.Now,
            Data = jsonUtil.Serialize(new {item = GetItemLogData(item), oldValue, newValue}),
        };
    }

    public CharacterHistoryEntryEntity CreateLogUseItemCharge(int characterId, ItemEntity item, int? oldValue, int? newValue)
    {
        return new CharacterHistoryEntryEntity
        {
            CharacterId = characterId,
            Action = UseChargeActionName,
            Date = DateTime.Now,
            Data = jsonUtil.Serialize(new {item = GetItemLogData(item), oldValue, newValue}),
        };
    }

    public CharacterHistoryEntryEntity CreateLogReadBook(int characterId, ItemEntity item)
    {
        return new CharacterHistoryEntryEntity
        {
            CharacterId = characterId,
            Action = ReadBookActionName,
            Date = DateTime.Now,
            Data = jsonUtil.Serialize(new {item = GetItemLogData(item)}),
        };
    }

    public CharacterHistoryEntryEntity CreateLogIdentifyItem(int characterId, ItemEntity item)
    {
        return new CharacterHistoryEntryEntity
        {
            CharacterId = characterId,
            Action = IdentifyActionName,
            Date = DateTime.Now,
            Data = jsonUtil.Serialize(new {item = GetItemLogData(item)}),
        };
    }

    public CharacterHistoryEntryEntity CreateLogLevelUp(int characterId, int level)
    {
        return new CharacterHistoryEntryEntity
        {
            CharacterId = characterId,
            Action = LevelUpActionName,
            Date = DateTime.Now,
            Data = jsonUtil.Serialize(new {level}),
        };
    }

    public CharacterHistoryEntryEntity CreateAddAptitude(int characterId, AptitudeEntity aptitude)
    {
        return new CharacterHistoryEntryEntity
        {
            CharacterId = characterId,
            Action = AddAptitudeActionName,
            Date = DateTime.Now,
            Data = jsonUtil.Serialize(new {aptitude = GetAptitudeLogData(aptitude)}),
        };
    }

    public CharacterHistoryEntryEntity CreateRemoveAptitude(int characterId, AptitudeEntity aptitude)
    {
        return new CharacterHistoryEntryEntity
        {
            CharacterId = characterId,
            Action = RemoveAptitudeActionName,
            Date = DateTime.Now,
            Data = jsonUtil.Serialize(new {aptitude = GetAptitudeLogData(aptitude)}),
        };
    }

    private object GetItemLogData(ItemEntity item)
    {
        var itemData = itemDataUtil.GetItemData(item);
        return new
        {
            name = itemData.Name,
            quantity = itemData.Quantity,
            icon = itemData.Icon,
        };
    }

    private static object GetModifierLogData(CharacterModifierEntity characterModifier)
    {
        return new {name = characterModifier.Name};
    }

    private static object GetAptitudeLogData(AptitudeEntity aptitude)
    {
        return new {id = aptitude.Id, name = aptitude.Name};
    }
}
