using System;
using System.Collections.Generic;
using Naheulbook.Core.Models;
using Naheulbook.Core.Notifications;
using Naheulbook.Data.Models;
using Naheulbook.Requests.Requests;
using Naheulbook.Shared.Utils;

namespace Naheulbook.Core.Utils
{
    public interface ICharacterUtil
    {
        void ApplyCharactersChange(NaheulbookExecutionContext executionContext, PatchCharacterRequest request, Character character, INotificationSession notificationSession);
        LevelUpResult LevelUpCharacter(Character character, Origin origin, List<Speciality> specialities, CharacterLevelUpRequest request);
    }

    public class CharacterUtil : ICharacterUtil
    {
        private readonly IAuthorizationUtil _authorizationUtil;
        private readonly IJsonUtil _jsonUtil;
        private readonly IOriginUtil _originUtil;
        private readonly ICharacterHistoryUtil _characterHistoryUtil;

        public CharacterUtil(
            IAuthorizationUtil authorizationUtil,
            ICharacterHistoryUtil characterHistoryUtil,
            IJsonUtil jsonUtil,
            IOriginUtil originUtil
        )
        {
            _authorizationUtil = authorizationUtil;
            _characterHistoryUtil = characterHistoryUtil;
            _jsonUtil = jsonUtil;
            _originUtil = originUtil;
        }

        public void ApplyCharactersChange(NaheulbookExecutionContext executionContext, PatchCharacterRequest request, Character character, INotificationSession notificationSession)
        {
            if (request.Debilibeuk.HasValue)
            {
                _authorizationUtil.EnsureIsGroupOwner(executionContext, character.Group);
                var gmData = _jsonUtil.Deserialize<CharacterGmData>(character.GmData) ?? new CharacterGmData();
                gmData.Debilibeuk = request.Debilibeuk.Value;
                character.GmData = _jsonUtil.Serialize(gmData);
                notificationSession.NotifyCharacterGmChangeData(character, gmData);
            }

            if (request.Mankdebol.HasValue)
            {
                _authorizationUtil.EnsureIsGroupOwner(executionContext, character.Group);
                var gmData = _jsonUtil.Deserialize<CharacterGmData>(character.GmData) ?? new CharacterGmData();
                gmData.Mankdebol = request.Mankdebol.Value;
                character.GmData = _jsonUtil.Serialize(gmData);
                notificationSession.NotifyCharacterGmChangeData(character, gmData);
            }

            if (request.IsActive.HasValue)
            {
                _authorizationUtil.EnsureIsGroupOwner(executionContext, character.Group);
                character.IsActive = request.IsActive.Value;
                notificationSession.NotifyCharacterGmChangeActive(character);
            }

            if (request.Color != null)
            {
                _authorizationUtil.EnsureIsGroupOwner(executionContext, character.Group);
                character.Color = request.Color;
                notificationSession.NotifyCharacterGmChangeColor(character);
            }

            if (request.OwnerId != null)
            {
                _authorizationUtil.EnsureIsGroupOwner(executionContext, character.Group);
                character.OwnerId = request.OwnerId.Value;
            }

            if (request.Target != null)
            {
                _authorizationUtil.EnsureIsGroupOwner(executionContext, character.Group);
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
                character.AddHistoryEntry(_characterHistoryUtil.CreateLogChangeEv(character, character.Ev, request.Ev));
                character.Ev = request.Ev;
                notificationSession.NotifyCharacterChangeEv(character);
            }

            if (request.Ea.HasValue)
            {
                character.AddHistoryEntry(_characterHistoryUtil.CreateLogChangeEa(character, character.Ea, request.Ea));
                character.Ea = request.Ea;
                notificationSession.NotifyCharacterChangeEa(character);
            }

            if (request.FatePoint.HasValue)
            {
                character.AddHistoryEntry(_characterHistoryUtil.CreateLogChangeFatePoint(character, character.FatePoint, request.FatePoint));
                character.FatePoint = request.FatePoint.Value;
                notificationSession.NotifyCharacterChangeFatePoint(character);
            }

            if (request.Experience.HasValue)
            {
                character.AddHistoryEntry(_characterHistoryUtil.CreateLogChangeExperience(character, character.Experience, request.Experience));
                character.Experience = request.Experience.Value;
                notificationSession.NotifyCharacterChangeExperience(character);
            }

            if (request.Sex != null)
            {
                character.AddHistoryEntry(_characterHistoryUtil.CreateLogChangeSex(character, character.Sex, request.Sex));
                character.Sex = request.Sex;
                notificationSession.NotifyCharacterChangeSex(character);
            }

            if (request.Name != null)
            {
                character.AddHistoryEntry(_characterHistoryUtil.CreateLogChangeName(character, character.Name, request.Name));
                character.Name = request.Name;
                notificationSession.NotifyCharacterChangeName(character);
            }

            if (request.Notes != null)
            {
                character.Notes = request.Notes;
                notificationSession.NotifyCharacterChangeNotes(character);
            }
        }

        public LevelUpResult LevelUpCharacter(Character character, Origin origin, List<Speciality> specialities, CharacterLevelUpRequest request)
        {
            var levelUpResult = new LevelUpResult();

            character.Level++;
            levelUpResult.NewLevel = character.Level;

            levelUpResult.NewModifiers.Add(CreateLevelUpCharacterModifier(character, request));

            if (request.SkillId.HasValue)
                levelUpResult.NewSkills.Add(CreateCharacterSkill(character, request.SkillId.Value));

            foreach (var speciality in specialities)
                levelUpResult.NewSpecialities.Add(CreateCharacterSpeciality(character, speciality));

            if ((character.Level == 2 || character.Level == 3) && _originUtil.HasFlag(origin, "CHA_+1_LVL2_LVL3"))
                levelUpResult.NewModifiers.Add(CreateChaLevelUpCharacterModifier(character));

            return levelUpResult;
        }

        private CharacterModifier CreateChaLevelUpCharacterModifier(Character character)
        {
            return new CharacterModifier
            {
                IsActive = true,
                CharacterId = character.Id,
                Permanent = true,
                DurationType = "forever",
                Name = "LevelUp charisme: " + character.Level,
                Values = new List<CharacterModifierValue>
                {
                    new CharacterModifierValue
                    {
                        StatName = "CHA",
                        Value = 1,
                        Type = "ADD"
                    }
                }
            };
        }

        private static CharacterSpeciality CreateCharacterSpeciality(Character character, Speciality speciality)
        {
            return new CharacterSpeciality
            {
                SpecialityId = speciality.Id,
                Speciality = speciality,
                CharacterId = character.Id
            };
        }

        private CharacterSkill CreateCharacterSkill(Character character, Guid skillId)
        {
            return new CharacterSkill
            {
                CharacterId = character.Id,
                SkillId = skillId
            };
        }

        private static CharacterModifier CreateLevelUpCharacterModifier(Character character, CharacterLevelUpRequest request)
        {
            var levelUpCharacterModifier = new CharacterModifier
            {
                IsActive = true,
                Name = "LevelUp: " + request.TargetLevelUp,
                Values = new List<CharacterModifierValue>(),
                Permanent = true,
                DurationType = "forever",
                CharacterId = character.Id
            };
            levelUpCharacterModifier.Values.Add(new CharacterModifierValue
            {
                StatName = request.EvOrEa,
                Value = request.EvOrEaValue,
                Type = "ADD"
            });
            levelUpCharacterModifier.Values.Add(new CharacterModifierValue
            {
                StatName = request.StatToUp,
                Value = 1,
                Type = "ADD"
            });
            return levelUpCharacterModifier;
        }
    }
}