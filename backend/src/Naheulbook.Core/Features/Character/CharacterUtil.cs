using Naheulbook.Core.Features.Origin;
using Naheulbook.Core.Features.Shared;
using Naheulbook.Core.Notifications;
using Naheulbook.Data.EntityFrameworkCore.Entities;
using Naheulbook.Requests.Requests;
using Naheulbook.Shared.Utils;

namespace Naheulbook.Core.Features.Character;

public interface ICharacterUtil
{
    void ApplyCharactersChange(NaheulbookExecutionContext executionContext, PatchCharacterRequest request, CharacterEntity character, INotificationSession notificationSession);
    LevelUpResult LevelUpCharacter(CharacterEntity character, OriginEntity origin, List<SpecialityEntity> specialities, CharacterLevelUpRequest request);
}

public class CharacterUtil(
    IAuthorizationUtil authorizationUtil,
    ICharacterHistoryUtil characterHistoryUtil,
    IJsonUtil jsonUtil,
    IOriginUtil originUtil
) : ICharacterUtil
{
    public void ApplyCharactersChange(NaheulbookExecutionContext executionContext, PatchCharacterRequest request, CharacterEntity character, INotificationSession notificationSession)
    {
        if (request.Debilibeuk.HasValue)
        {
            authorizationUtil.EnsureIsGroupOwner(executionContext, character.Group);
            var gmData = jsonUtil.Deserialize<CharacterGmData>(character.GmData) ?? new CharacterGmData();
            gmData.Debilibeuk = request.Debilibeuk.Value;
            character.GmData = jsonUtil.Serialize(gmData);
            notificationSession.NotifyCharacterGmChangeData(character, gmData);
        }

        if (request.Mankdebol.HasValue)
        {
            authorizationUtil.EnsureIsGroupOwner(executionContext, character.Group);
            var gmData = jsonUtil.Deserialize<CharacterGmData>(character.GmData) ?? new CharacterGmData();
            gmData.Mankdebol = request.Mankdebol.Value;
            character.GmData = jsonUtil.Serialize(gmData);
            notificationSession.NotifyCharacterGmChangeData(character, gmData);
        }

        if (request.IsActive.HasValue)
        {
            authorizationUtil.EnsureIsGroupOwner(executionContext, character.Group);
            character.IsActive = request.IsActive.Value;
            notificationSession.NotifyCharacterGmChangeActive(character);
        }

        if (request.Color != null)
        {
            authorizationUtil.EnsureIsGroupOwner(executionContext, character.Group);
            character.Color = request.Color;
            notificationSession.NotifyCharacterGmChangeColor(character);
        }

        if (request.OwnerId != null)
        {
            authorizationUtil.EnsureIsGroupOwner(executionContext, character.Group);
            character.OwnerId = request.OwnerId.Value;
        }

        if (request.Target != null)
        {
            authorizationUtil.EnsureIsGroupOwner(executionContext, character.Group);
            if (request.Target.IsMonster)
            {
                character.TargetedCharacterId = null;
                character.TargetedMonsterId = request.Target.Id;
            }
            else
            {
                character.TargetedMonsterId = null;
                character.TargetedCharacterId = request.Target.Id;
            }

            notificationSession.NotifyCharacterGmChangeTarget(character, request.Target);
        }

        if (request.Ev.HasValue)
        {
            character.AddHistoryEntry(characterHistoryUtil.CreateLogChangeEv(character, character.Ev, request.Ev));
            character.Ev = request.Ev;
            notificationSession.NotifyCharacterChangeEv(character);
        }

        if (request.Ea.HasValue)
        {
            character.AddHistoryEntry(characterHistoryUtil.CreateLogChangeEa(character, character.Ea, request.Ea));
            character.Ea = request.Ea;
            notificationSession.NotifyCharacterChangeEa(character);
        }

        if (request.FatePoint.HasValue)
        {
            character.AddHistoryEntry(characterHistoryUtil.CreateLogChangeFatePoint(character, character.FatePoint, request.FatePoint));
            character.FatePoint = request.FatePoint.Value;
            notificationSession.NotifyCharacterChangeFatePoint(character);
        }

        if (request.Experience.HasValue)
        {
            character.AddHistoryEntry(characterHistoryUtil.CreateLogChangeExperience(character, character.Experience, request.Experience));
            character.Experience = request.Experience.Value;
            notificationSession.NotifyCharacterChangeExperience(character);
        }

        if (request.Sex != null)
        {
            character.AddHistoryEntry(characterHistoryUtil.CreateLogChangeSex(character, character.Sex, request.Sex));
            character.Sex = request.Sex;
            notificationSession.NotifyCharacterChangeSex(character);
        }

        if (request.Name != null)
        {
            character.AddHistoryEntry(characterHistoryUtil.CreateLogChangeName(character, character.Name, request.Name));
            character.Name = request.Name;
            notificationSession.NotifyCharacterChangeName(character);
        }

        if (request.Notes != null)
        {
            character.Notes = request.Notes;
            notificationSession.NotifyCharacterChangeNotes(character);
        }
    }

    public LevelUpResult LevelUpCharacter(CharacterEntity character, OriginEntity origin, List<SpecialityEntity> specialities, CharacterLevelUpRequest request)
    {
        var levelUpResult = new LevelUpResult();

        character.Level++;
        levelUpResult.NewLevel = character.Level;

        levelUpResult.NewModifiers.Add(CreateLevelUpCharacterModifier(character, request));

        if (request.SkillId.HasValue)
            levelUpResult.NewSkills.Add(CreateCharacterSkill(character, request.SkillId.Value));

        foreach (var speciality in specialities)
            levelUpResult.NewSpecialities.Add(CreateCharacterSpeciality(character, speciality));

        if ((character.Level == 2 || character.Level == 3) && originUtil.HasFlag(origin, "CHA_+1_LVL2_LVL3"))
            levelUpResult.NewModifiers.Add(CreateChaLevelUpCharacterModifier(character));

        return levelUpResult;
    }

    private CharacterModifierEntity CreateChaLevelUpCharacterModifier(CharacterEntity character)
    {
        return new CharacterModifierEntity
        {
            IsActive = true,
            CharacterId = character.Id,
            Permanent = true,
            DurationType = "forever",
            Name = "LevelUp charisme: " + character.Level,
            Values = new List<CharacterModifierValueEntity>
            {
                new()
                {
                    StatName = "CHA",
                    Value = 1,
                    Type = "ADD",
                },
            },
        };
    }

    private static CharacterSpecialityEntity CreateCharacterSpeciality(CharacterEntity character, SpecialityEntity speciality)
    {
        return new CharacterSpecialityEntity
        {
            SpecialityId = speciality.Id,
            Speciality = speciality,
            CharacterId = character.Id,
        };
    }

    private CharacterSkillEntity CreateCharacterSkill(CharacterEntity character, Guid skillId)
    {
        return new CharacterSkillEntity
        {
            CharacterId = character.Id,
            SkillId = skillId,
        };
    }

    private static CharacterModifierEntity CreateLevelUpCharacterModifier(CharacterEntity character, CharacterLevelUpRequest request)
    {
        var levelUpCharacterModifier = new CharacterModifierEntity
        {
            IsActive = true,
            Name = "LevelUp: " + request.TargetLevelUp,
            Values = new List<CharacterModifierValueEntity>(),
            Permanent = true,
            DurationType = "forever",
            CharacterId = character.Id,
        };
        levelUpCharacterModifier.Values.Add(new CharacterModifierValueEntity
        {
            StatName = request.EvOrEa,
            Value = request.EvOrEaValue,
            Type = "ADD",
        });
        levelUpCharacterModifier.Values.Add(new CharacterModifierValueEntity
        {
            StatName = request.StatToUp,
            Value = 1,
            Type = "ADD",
        });
        return levelUpCharacterModifier;
    }
}