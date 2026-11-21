using Newtonsoft.Json.Linq;
using Naheulbook.Data.EntityFrameworkCore.Entities;
using Naheulbook.Shared.Utils;

namespace Naheulbook.Core.Features.Character;

public interface ICharacterHistoryUtil
{
    CharacterHistoryEntryEntity CreateLogChangeEv(CharacterEntity character, int? oldValue, int? newValue);
    CharacterHistoryEntryEntity CreateLogChangeEa(CharacterEntity character, int? oldValue, int? newValue);
    CharacterHistoryEntryEntity CreateLogChangeFatePoint(CharacterEntity character, short oldValue, short? newValue);
    CharacterHistoryEntryEntity CreateLogChangeExperience(CharacterEntity character, int oldValue, int? newValue, string? info = null);
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
    CharacterHistoryEntryEntity CreateAddSpecialAttack(int characterId, SpecialAttackEntity specialAttack);
    CharacterHistoryEntryEntity CreateRemoveSpecialAttack(int characterId, SpecialAttackEntity specialAttack);
    CharacterHistoryEntryEntity CreateUseSpecialAttack(int characterId, SpecialAttackEntity specialAttack, bool success);
    CharacterHistoryEntryEntity CreateAddTalent(int characterId, TalentEntity talent);
    CharacterHistoryEntryEntity CreateRemoveTalent(int characterId, TalentEntity talent);
    CharacterHistoryEntryEntity CreateAddMastery(int characterId, MasteryEntity mastery);
    CharacterHistoryEntryEntity CreateRemoveMastery(int characterId, MasteryEntity mastery);
    CharacterHistoryEntryEntity CreateAddHandicap(int characterId, HandicapEntity handicap);
    CharacterHistoryEntryEntity CreateRemoveHandicap(int characterId, HandicapEntity handicap);
    CharacterHistoryEntryEntity CreateAddSpellBook(int characterId, SpellBookEntity spellBook, int? maxLevel);
    CharacterHistoryEntryEntity CreateUpdateSpellBook(int characterId, SpellBookEntity spellBook, int? oldMaxLevel, int? newMaxLevel);
    CharacterHistoryEntryEntity CreateRemoveSpellBook(int characterId, SpellBookEntity spellBook);
}

public class CharacterHistoryUtil(IJsonUtil jsonUtil) : ICharacterHistoryUtil
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
    private const string AddSpecialAttackActionName = "ADD_SPECIAL_ATTACK";
    private const string RemoveSpecialAttackActionName = "REMOVE_SPECIAL_ATTACK";
    private const string UseSpecialAttackActionName = "USE_SPECIAL_ATTACK";
    private const string AddTalentActionName = "ADD_TALENT";
    private const string RemoveTalentActionName = "REMOVE_TALENT";
    private const string AddMasteryActionName = "ADD_MASTERY";
    private const string RemoveMasteryActionName = "REMOVE_MASTERY";
    private const string AddHandicapActionName = "ADD_HANDICAP";
    private const string AddSpellBookActionName = "ADD_SPELL_BOOK";
    private const string UpdateSpellBookActionName = "UPDATE_SPELL_BOOK";
    private const string RemoveSpellBookActionName = "REMOVE_SPELL_BOOK";
    private const string RemoveHandicapActionName = "REMOVE_HANDICAP";

    public CharacterHistoryEntryEntity CreateLogChangeEv(CharacterEntity character, int? oldValue, int? newValue)
    {
        return new CharacterHistoryEntryEntity
        {
            Character = character,
            Action = ChangeEvActionName,
            Date = DateTime.UtcNow,
            Data = jsonUtil.Serialize(new {oldValue, newValue}),
        };
    }

    public CharacterHistoryEntryEntity CreateLogChangeEa(CharacterEntity character, int? oldValue, int? newValue)
    {
        return new CharacterHistoryEntryEntity
        {
            Character = character,
            Action = ChangeEaActionName,
            Date = DateTime.UtcNow,
            Data = jsonUtil.Serialize(new {oldValue, newValue}),
        };
    }

    public CharacterHistoryEntryEntity CreateLogChangeFatePoint(CharacterEntity character, short oldValue, short? newValue)
    {
        return new CharacterHistoryEntryEntity
        {
            Character = character,
            Action = ChangeFatePointActionName,
            Date = DateTime.UtcNow,
            Data = jsonUtil.Serialize(new {oldValue, newValue}),
        };
    }

    public CharacterHistoryEntryEntity CreateLogChangeExperience(CharacterEntity character, int oldValue, int? newValue, string? info = null)
    {
        return new CharacterHistoryEntryEntity
        {
            Character = character,
            Action = ChangeExperienceActionName,
            Date = DateTime.UtcNow,
            Data = jsonUtil.Serialize(new {oldValue, newValue, info}),
        };
    }

    public CharacterHistoryEntryEntity CreateLogChangeSex(CharacterEntity character, string oldValue, string newValue)
    {
        return new CharacterHistoryEntryEntity
        {
            Character = character,
            Action = ChangeSexActionName,
            Date = DateTime.UtcNow,
            Data = jsonUtil.Serialize(new {oldValue, newValue}),
        };
    }

    public CharacterHistoryEntryEntity CreateLogChangeName(CharacterEntity character, string oldValue, string newValue)
    {
        return new CharacterHistoryEntryEntity
        {
            Character = character,
            Action = ChangeNameActionName,
            Date = DateTime.UtcNow,
            Data = jsonUtil.Serialize(new {oldValue, newValue}),
        };
    }

    public CharacterHistoryEntryEntity CreateLogAddItem(int characterId, ItemEntity item)
    {
        return new CharacterHistoryEntryEntity
        {
            CharacterId = characterId,
            Action = AddItemActionName,
            Date = DateTime.UtcNow,
            Data = jsonUtil.Serialize(new {item = GetItemLogData(item)}),
        };
    }

    public CharacterHistoryEntryEntity CreateLogEquipItem(int characterId, ItemEntity item)
    {
        return new CharacterHistoryEntryEntity
        {
            CharacterId = characterId,
            Action = EquipActionName,
            Date = DateTime.UtcNow,
            Data = jsonUtil.Serialize(new {item = GetItemLogData(item)}),
        };
    }

    public CharacterHistoryEntryEntity CreateLogUnEquipItem(int characterId, ItemEntity item)
    {
        return new CharacterHistoryEntryEntity
        {
            CharacterId = characterId,
            Action = UnEquipActionName,
            Date = DateTime.UtcNow,
            Data = jsonUtil.Serialize(new {item = GetItemLogData(item)}),
        };
    }

    public CharacterHistoryEntryEntity CreateLogAddModifier(CharacterEntity character, CharacterModifierEntity characterModifier)
    {
        return new CharacterHistoryEntryEntity
        {
            CharacterId = character.Id,
            Action = ApplyModifierActionName,
            Date = DateTime.UtcNow,
            Data = jsonUtil.Serialize(new {modifier = GetModifierLogData(characterModifier)}),
        };
    }

    public CharacterHistoryEntryEntity CreateLogRemoveModifier(int characterId, CharacterModifierEntity characterModifier)
    {
        return new CharacterHistoryEntryEntity
        {
            CharacterId = characterId,
            Action = RemoveModifierActionName,
            Date = DateTime.UtcNow,
            Data = jsonUtil.Serialize(new {modifier = GetModifierLogData(characterModifier)}),
        };
    }

    public CharacterHistoryEntryEntity CreateLogActiveModifier(int characterId, CharacterModifierEntity characterModifier)
    {
        return new CharacterHistoryEntryEntity
        {
            CharacterId = characterId,
            Action = ActiveModifierActionName,
            Date = DateTime.UtcNow,
            Data = jsonUtil.Serialize(new {modifier = GetModifierLogData(characterModifier)}),
        };
    }

    public CharacterHistoryEntryEntity CreateLogDisableModifier(int characterId, CharacterModifierEntity characterModifier)
    {
        return new CharacterHistoryEntryEntity
        {
            CharacterId = characterId,
            Action = DisableModifierActionName,
            Date = DateTime.UtcNow,
            Data = jsonUtil.Serialize(new {modifier = GetModifierLogData(characterModifier)}),
        };
    }

    public CharacterHistoryEntryEntity CreateLogGiveItem(int characterId, ItemEntity item, string? targetCharacterName)
    {
        return new CharacterHistoryEntryEntity
        {
            CharacterId = characterId,
            Action = GiveItemActionName,
            Date = DateTime.UtcNow,
            Data = jsonUtil.Serialize(new {item = GetItemLogData(item), characterName = targetCharacterName}),
        };
    }

    public CharacterHistoryEntryEntity CreateLogGivenItem(int characterId, ItemEntity item, string? sourceCharacterName)
    {
        return new CharacterHistoryEntryEntity
        {
            CharacterId = characterId,
            Action = GivenItemActionName,
            Date = DateTime.UtcNow,
            Data = jsonUtil.Serialize(new {item = GetItemLogData(item), characterName = sourceCharacterName}),
        };
    }

    public CharacterHistoryEntryEntity CreateLogLootItem(int characterId, ItemEntity item)
    {
        return new CharacterHistoryEntryEntity
        {
            CharacterId = characterId,
            Action = LootItemActionName,
            Date = DateTime.UtcNow,
            Data = jsonUtil.Serialize(new {item = GetItemLogData(item)}),
        };
    }

    public CharacterHistoryEntryEntity CreateLogChangeItemQuantity(int characterId, ItemEntity item, int? oldValue, int? newValue)
    {
        return new CharacterHistoryEntryEntity
        {
            CharacterId = characterId,
            Action = ChangeQuantityActionName,
            Date = DateTime.UtcNow,
            Data = jsonUtil.Serialize(new {item = GetItemLogData(item), oldValue, newValue}),
        };
    }

    public CharacterHistoryEntryEntity CreateLogUseItemCharge(int characterId, ItemEntity item, int? oldValue, int? newValue)
    {
        return new CharacterHistoryEntryEntity
        {
            CharacterId = characterId,
            Action = UseChargeActionName,
            Date = DateTime.UtcNow,
            Data = jsonUtil.Serialize(new {item = GetItemLogData(item), oldValue, newValue}),
        };
    }

    public CharacterHistoryEntryEntity CreateLogReadBook(int characterId, ItemEntity item)
    {
        return new CharacterHistoryEntryEntity
        {
            CharacterId = characterId,
            Action = ReadBookActionName,
            Date = DateTime.UtcNow,
            Data = jsonUtil.Serialize(new {item = GetItemLogData(item)}),
        };
    }

    public CharacterHistoryEntryEntity CreateLogIdentifyItem(int characterId, ItemEntity item)
    {
        return new CharacterHistoryEntryEntity
        {
            CharacterId = characterId,
            Action = IdentifyActionName,
            Date = DateTime.UtcNow,
            Data = jsonUtil.Serialize(new {item = GetItemLogData(item)}),
        };
    }

    public CharacterHistoryEntryEntity CreateLogLevelUp(int characterId, int level)
    {
        return new CharacterHistoryEntryEntity
        {
            CharacterId = characterId,
            Action = LevelUpActionName,
            Date = DateTime.UtcNow,
            Data = jsonUtil.Serialize(new {level}),
        };
    }

    public CharacterHistoryEntryEntity CreateAddAptitude(int characterId, AptitudeEntity aptitude)
    {
        return new CharacterHistoryEntryEntity
        {
            CharacterId = characterId,
            Action = AddAptitudeActionName,
            Date = DateTime.UtcNow,
            Data = jsonUtil.Serialize(new {aptitude = GetAptitudeLogData(aptitude)}),
        };
    }

    public CharacterHistoryEntryEntity CreateRemoveAptitude(int characterId, AptitudeEntity aptitude)
    {
        return new CharacterHistoryEntryEntity
        {
            CharacterId = characterId,
            Action = RemoveAptitudeActionName,
            Date = DateTime.UtcNow,
            Data = jsonUtil.Serialize(new {aptitude = GetAptitudeLogData(aptitude)}),
        };
    }

    public CharacterHistoryEntryEntity CreateAddSpecialAttack(int characterId, SpecialAttackEntity specialAttack)
    {
        return new CharacterHistoryEntryEntity
        {
            CharacterId = characterId,
            Action = AddSpecialAttackActionName,
            Date = DateTime.UtcNow,
            Data = jsonUtil.Serialize(new {specialAttack = GetSpecialAttackLogData(specialAttack)}),
        };
    }

    public CharacterHistoryEntryEntity CreateRemoveSpecialAttack(int characterId, SpecialAttackEntity specialAttack)
    {
        return new CharacterHistoryEntryEntity
        {
            CharacterId = characterId,
            Action = RemoveSpecialAttackActionName,
            Date = DateTime.UtcNow,
            Data = jsonUtil.Serialize(new {specialAttack = GetSpecialAttackLogData(specialAttack)}),
        };
    }

    public CharacterHistoryEntryEntity CreateUseSpecialAttack(int characterId, SpecialAttackEntity specialAttack, bool success)
    {
        return new CharacterHistoryEntryEntity
        {
            CharacterId = characterId,
            Action = UseSpecialAttackActionName,
            Date = DateTime.UtcNow,
            Data = jsonUtil.Serialize(new {specialAttack = GetSpecialAttackLogData(specialAttack), success}),
        };
    }

    public CharacterHistoryEntryEntity CreateAddTalent(int characterId, TalentEntity talent)
    {
        return new CharacterHistoryEntryEntity
        {
            CharacterId = characterId,
            Action = AddTalentActionName,
            Date = DateTime.UtcNow,
            Data = jsonUtil.Serialize(new {talent = new {id = talent.Id, name = talent.Name}}),
        };
    }

    public CharacterHistoryEntryEntity CreateRemoveTalent(int characterId, TalentEntity talent)
    {
        return new CharacterHistoryEntryEntity
        {
            CharacterId = characterId,
            Action = RemoveTalentActionName,
            Date = DateTime.UtcNow,
            Data = jsonUtil.Serialize(new {talent = new {id = talent.Id, name = talent.Name}}),
        };
    }

    public CharacterHistoryEntryEntity CreateAddMastery(int characterId, MasteryEntity mastery)
    {
        return new CharacterHistoryEntryEntity
        {
            CharacterId = characterId,
            Action = AddMasteryActionName,
            Date = DateTime.UtcNow,
            Data = jsonUtil.Serialize(new {mastery = new {id = mastery.Id, name = mastery.Name}}),
        };
    }

    public CharacterHistoryEntryEntity CreateRemoveMastery(int characterId, MasteryEntity mastery)
    {
        return new CharacterHistoryEntryEntity
        {
            CharacterId = characterId,
            Action = RemoveMasteryActionName,
            Date = DateTime.UtcNow,
            Data = jsonUtil.Serialize(new {mastery = new {id = mastery.Id, name = mastery.Name}}),
        };
    }

    public CharacterHistoryEntryEntity CreateAddHandicap(int characterId, HandicapEntity handicap)
    {
        return new CharacterHistoryEntryEntity
        {
            CharacterId = characterId,
            Action = AddHandicapActionName,
            Date = DateTime.UtcNow,
            Data = jsonUtil.Serialize(new {handicap = new {id = handicap.Id, name = handicap.Name}}),
        };
    }

    public CharacterHistoryEntryEntity CreateRemoveHandicap(int characterId, HandicapEntity handicap)
    {
        return new CharacterHistoryEntryEntity
        {
            CharacterId = characterId,
            Action = RemoveHandicapActionName,
            Date = DateTime.UtcNow,
            Data = jsonUtil.Serialize(new {handicap = new {id = handicap.Id, name = handicap.Name}}),
        };
    }

    /// <summary>
    /// Snapshot of the item, so the history can display it as it was at the time of the log, even after it was deleted
    /// </summary>
    private object GetItemLogData(ItemEntity item)
    {
        return new
        {
            templateId = item.ItemTemplateId,
            data = jsonUtil.Deserialize<JObject>(item.Data) ?? new JObject(),
            modifiers = jsonUtil.Deserialize<JArray>(item.Modifiers),
        };
    }

    private static object GetModifierLogData(CharacterModifierEntity characterModifier)
    {
        return new
        {
            name = characterModifier.Name,
            values = characterModifier.Values.Select(v => new {stat = v.StatName, type = v.Type, value = v.Value}),
        };
    }

    private static object GetAptitudeLogData(AptitudeEntity aptitude)
    {
        return new {id = aptitude.Id, name = aptitude.Name};
    }

    private static object GetSpecialAttackLogData(SpecialAttackEntity specialAttack)
    {
        return new {id = specialAttack.Id, name = specialAttack.Name};
    }

    public CharacterHistoryEntryEntity CreateAddSpellBook(int characterId, SpellBookEntity spellBook, int? maxLevel)
    {
        return new CharacterHistoryEntryEntity
        {
            CharacterId = characterId,
            Action = AddSpellBookActionName,
            Date = DateTime.UtcNow,
            Data = jsonUtil.Serialize(new {spellBook = new {id = spellBook.Id, name = spellBook.Name}, maxLevel}),
        };
    }

    public CharacterHistoryEntryEntity CreateUpdateSpellBook(int characterId, SpellBookEntity spellBook, int? oldMaxLevel, int? newMaxLevel)
    {
        return new CharacterHistoryEntryEntity
        {
            CharacterId = characterId,
            Action = UpdateSpellBookActionName,
            Date = DateTime.UtcNow,
            Data = jsonUtil.Serialize(new {spellBook = new {id = spellBook.Id, name = spellBook.Name}, oldMaxLevel, newMaxLevel}),
        };
    }

    public CharacterHistoryEntryEntity CreateRemoveSpellBook(int characterId, SpellBookEntity spellBook)
    {
        return new CharacterHistoryEntryEntity
        {
            CharacterId = characterId,
            Action = RemoveSpellBookActionName,
            Date = DateTime.UtcNow,
            Data = jsonUtil.Serialize(new {spellBook = new {id = spellBook.Id, name = spellBook.Name}}),
        };
    }
}
