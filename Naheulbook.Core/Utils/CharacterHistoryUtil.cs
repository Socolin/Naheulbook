using System;
using Naheulbook.Data.Models;
using Naheulbook.Shared.TransientModels;
using Naheulbook.Shared.Utils;

namespace Naheulbook.Core.Utils
{
    public interface ICharacterHistoryUtil
    {
        CharacterHistoryEntry CreateLogChangeEv(Character character, int? oldValue, int? newValue);
        CharacterHistoryEntry CreateLogChangeEa(Character character, int? oldValue, int? newValue);
        CharacterHistoryEntry CreateLogChangeFatePoint(Character character, short oldValue, short? newValue);
        CharacterHistoryEntry CreateLogChangeExperience(Character character, int oldValue, int? newValue);
        CharacterHistoryEntry CreateLogChangeSex(Character character, string oldValue, string newValue);
        CharacterHistoryEntry CreateLogChangeName(Character character, string oldValue, string newValue);
        CharacterHistoryEntry CreateLogAddItem(int characterId, Item item);
        CharacterHistoryEntry CreateLogEquipItem(int characterId, Item item);
        CharacterHistoryEntry CreateLogUnEquipItem(int characterId, Item item);
        CharacterHistoryEntry CreateLogAddModifier(Character character, CharacterModifier characterModifier);
        CharacterHistoryEntry CreateLogRemoveModifier(int characterId, CharacterModifier characterModifier);
        CharacterHistoryEntry CreateLogActiveModifier(int characterId, CharacterModifier characterModifier);
        CharacterHistoryEntry CreateLogDisableModifier(int characterId, CharacterModifier characterModifier);
        CharacterHistoryEntry CreateLogGiveItem(int characterId, Item item, string destCharacterName);
        CharacterHistoryEntry CreateLogGivenItem(int characterId, Item item, string? sourceCharacterName);
        CharacterHistoryEntry CreateLogLootItem(int characterId, Item item);
        CharacterHistoryEntry CreateLogChangeItemQuantity(int characterId, Item item, int? oldValue, int? newValue);
        CharacterHistoryEntry CreateLogUseItemCharge(int characterId, Item item, int? oldValue, int? newValue);
        CharacterHistoryEntry CreateLogReadBook(int characterId, Item item);
        CharacterHistoryEntry CreateLogIdentifyItem(int characterId, Item item);
        CharacterHistoryEntry CreateLogLevelUp(int characterId, int characterLevel);
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
        private readonly IItemDataUtil _itemDataUtil;

        public CharacterHistoryUtil(
            IJsonUtil jsonUtil,
            IItemDataUtil itemDataUtil
        )
        {
            _jsonUtil = jsonUtil;
            _itemDataUtil = itemDataUtil;
        }

        public CharacterHistoryEntry CreateLogChangeEv(Character character, int? oldValue, int? newValue)
        {
            return new CharacterHistoryEntry
            {
                Character = character,
                Action = ChangeEvActionName,
                Date = DateTime.Now,
                Data = _jsonUtil.Serialize(new {oldValue, newValue})
            };
        }

        public CharacterHistoryEntry CreateLogChangeEa(Character character, int? oldValue, int? newValue)
        {
            return new CharacterHistoryEntry
            {
                Character = character,
                Action = ChangeEaActionName,
                Date = DateTime.Now,
                Data = _jsonUtil.Serialize(new {oldValue, newValue})
            };
        }

        public CharacterHistoryEntry CreateLogChangeFatePoint(Character character, short oldValue, short? newValue)
        {
            return new CharacterHistoryEntry
            {
                Character = character,
                Action = ChangeFatePointActionName,
                Date = DateTime.Now,
                Data = _jsonUtil.Serialize(new {oldValue, newValue})
            };
        }

        public CharacterHistoryEntry CreateLogChangeExperience(Character character, int oldValue, int? newValue)
        {
            return new CharacterHistoryEntry
            {
                Character = character,
                Action = ChangeExperienceActionName,
                Date = DateTime.Now,
                Data = _jsonUtil.Serialize(new {oldValue, newValue})
            };
        }

        public CharacterHistoryEntry CreateLogChangeSex(Character character, string oldValue, string newValue)
        {
            return new CharacterHistoryEntry
            {
                Character = character,
                Action = ChangeSexActionName,
                Date = DateTime.Now,
                Data = _jsonUtil.Serialize(new {oldValue, newValue})
            };
        }

        public CharacterHistoryEntry CreateLogChangeName(Character character, string oldValue, string newValue)
        {
            return new CharacterHistoryEntry
            {
                Character = character,
                Action = ChangeNameActionName,
                Date = DateTime.Now,
                Data = _jsonUtil.Serialize(new {oldValue, newValue})
            };
        }

        public CharacterHistoryEntry CreateLogAddItem(int characterId, Item item)
        {
            return new CharacterHistoryEntry
            {
                CharacterId = characterId,
                Action = AddItemActionName,
                Date = DateTime.Now,
                Data = GetItemLogData(item)
            };
        }

        public CharacterHistoryEntry CreateLogEquipItem(int characterId, Item item)
        {
            return new CharacterHistoryEntry
            {
                CharacterId = characterId,
                Action = EquipActionName,
                Date = DateTime.Now,
                Data = GetItemLogData(item)
            };
        }

        public CharacterHistoryEntry CreateLogUnEquipItem(int characterId, Item item)
        {
            return new CharacterHistoryEntry
            {
                CharacterId = characterId,
                Action = UnEquipActionName,
                Date = DateTime.Now,
                Data = GetItemLogData(item)
            };
        }

        public CharacterHistoryEntry CreateLogAddModifier(Character character, CharacterModifier characterModifier)
        {
            return new CharacterHistoryEntry
            {
                CharacterId = character.Id,
                Action = ApplyModifierActionName,
                Date = DateTime.Now,
                Data = GetCharacterModifierLogData(characterModifier)
            };
        }

        public CharacterHistoryEntry CreateLogRemoveModifier(int characterId, CharacterModifier characterModifier)
        {
            return new CharacterHistoryEntry
            {
                CharacterId = characterId,
                Action = RemoveModifierActionName,
                Date = DateTime.Now,
                Data = GetCharacterModifierLogData(characterModifier)
            };
        }

        public CharacterHistoryEntry CreateLogActiveModifier(int characterId, CharacterModifier characterModifier)
        {
            return new CharacterHistoryEntry
            {
                CharacterId = characterId,
                Action = ActiveModifierActionName,
                Date = DateTime.Now,
                Data = GetCharacterModifierLogData(characterModifier)
            };
        }

        public CharacterHistoryEntry CreateLogDisableModifier(int characterId, CharacterModifier characterModifier)
        {
            return new CharacterHistoryEntry
            {
                CharacterId = characterId,
                Action = DisableModifierActionName,
                Date = DateTime.Now,
                Data = GetCharacterModifierLogData(characterModifier)
            };
        }

        public CharacterHistoryEntry CreateLogGiveItem(int characterId, Item item, string destCharacterName)
        {
            return new CharacterHistoryEntry
            {
                CharacterId = characterId,
                Action = GiveItemActionName,
                Date = DateTime.Now,
                Data = GetItemGiveLogData(item, destCharacterName)
            };
        }

        public CharacterHistoryEntry CreateLogGivenItem(int characterId, Item item, string? sourceCharacterName)
        {
            return new CharacterHistoryEntry
            {
                CharacterId = characterId,
                Action = GivenItemActionName,
                Date = DateTime.Now,
                Data = GetItemGiveLogData(item, sourceCharacterName)
            };
        }

        public CharacterHistoryEntry CreateLogLootItem(int characterId, Item item)
        {
            return new CharacterHistoryEntry
            {
                CharacterId = characterId,
                Action = LootItemActionName,
                Date = DateTime.Now,
                Data = GetItemLogData(item)
            };
        }

        public CharacterHistoryEntry CreateLogChangeItemQuantity(int characterId, Item item, int? oldValue, int? newValue)
        {
            return new CharacterHistoryEntry
            {
                CharacterId = characterId,
                Action = ChangeQuantityActionName,
                Date = DateTime.Now,
                Data = GetItemQuantityChangeDataLog(item, oldValue, newValue)
            };
        }

        public CharacterHistoryEntry CreateLogUseItemCharge(int characterId, Item item, int? oldValue, int? newValue)
        {
            return new CharacterHistoryEntry
            {
                CharacterId = characterId,
                Action = UseChargeActionName,
                Date = DateTime.Now,
                Data = GetItemQuantityChangeDataLog(item, oldValue, newValue)
            };
        }

        public CharacterHistoryEntry CreateLogReadBook(int characterId, Item item)
        {
            return new CharacterHistoryEntry
            {
                CharacterId = characterId,
                Action = ReadBookActionName,
                Date = DateTime.Now,
                Data = GetItemLogData(item)
            };
        }

        public CharacterHistoryEntry CreateLogIdentifyItem(int characterId, Item item)
        {
            return new CharacterHistoryEntry
            {
                CharacterId = characterId,
                Action = IdentifyActionName,
                Date = DateTime.Now,
                Data = GetItemLogData(item)
            };
        }

        public CharacterHistoryEntry CreateLogLevelUp(int characterId, int level)
        {
            return new CharacterHistoryEntry
            {
                CharacterId = characterId,
                Action = LevelUpActionName,
                Date = DateTime.Now,
                Data = _jsonUtil.SerializeNonNull(new {level})
            };
        }

        private string? GetItemLogData(Item item)
        {
            var itemData = _itemDataUtil.GetItemData(item);
            var itemLogData = _jsonUtil.Serialize(new ItemLogData
            {
                Name = itemData.Name ?? item.ItemTemplate?.Name,
                Quantity = itemData.Quantity,
                Ug = itemData.Ug,
                Icon = itemData.Icon
            });
            return itemLogData;
        }

        private string? GetItemQuantityChangeDataLog(Item item, int? oldValue, int? newValue)
        {
            var itemData = _itemDataUtil.GetItemData(item);
            var itemLogData = _jsonUtil.Serialize(new ItemQuantityChangeLogData
            {
                Name = itemData.Name ?? item.ItemTemplate?.Name,
                Quantity = itemData.Quantity,
                Ug = itemData.Ug,
                Icon = itemData.Icon,
                NewValue = newValue,
                OldValue = oldValue
            });
            return itemLogData;
        }


        private string? GetItemGiveLogData(Item item, string? characterName)
        {
            var itemData = _itemDataUtil.GetItemData(item);
            var itemLogData = _jsonUtil.Serialize(new ItemGiveLogData
            {
                Name = itemData.Name ?? item.ItemTemplate?.Name,
                Quantity = itemData.Quantity,
                Ug = itemData.Ug,
                Icon = itemData.Icon,
                CharacterName = characterName
            });
            return itemLogData;
        }

        private string? GetCharacterModifierLogData(CharacterModifier modifier)
        {
            var itemLogData = _jsonUtil.Serialize(new CharacterModifierLogData
            {
                Name = modifier.Name
            });
            return itemLogData;
        }
    }
}