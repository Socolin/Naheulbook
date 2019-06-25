using System;
using Naheulbook.Data.Models;
using Naheulbook.Shared.Utils;

namespace Naheulbook.Core.Utils
{
    public interface ICharacterHistoryUtil
    {
        CharacterHistoryEntry CreateLogChangeEv(Character character, int? characterEv, int? requestEv);
        CharacterHistoryEntry CreateLogChangeEa(Character character, int? characterEa, int? requestEa);
        CharacterHistoryEntry CreateLogChangeFatePoint(Character character, short characterFatePoint, short? requestFatePoint);
        CharacterHistoryEntry CreateLogChangeExperience(Character character, int characterExperience, int? requestExperience);
        CharacterHistoryEntry CreateLogChangeSex(Character character, string characterSex, string requestSex);
        CharacterHistoryEntry CreateLogChangeName(Character character, string characterName, string requestName);
        CharacterHistoryEntry CreateLogAddItem(int characterId, Item item);
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


        private readonly IJsonUtil _jsonUtil;

        public CharacterHistoryUtil(IJsonUtil jsonUtil)
        {
            _jsonUtil = jsonUtil;
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
                ItemId = item.Id
            };
        }
    }
}