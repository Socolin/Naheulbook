using System;
using System.Collections.Generic;
using Naheulbook.Data.Models;

namespace Naheulbook.TestUtils
{
    public partial class TestDataUtil
    {
        public TestDataUtil AddCharacter(int ownerId, Action<CharacterEntity> customizer = null)
        {
            return SaveEntity(_defaultEntityCreator.CreateCharacter(ownerId, GetLast<OriginEntity>()), customizer);
        }

        public TestDataUtil AddCharacter(Action<CharacterEntity> customizer = null)
        {
            return AddCharacter(out _, customizer);
        }

        public TestDataUtil AddCharacter(out CharacterEntity character, Action<CharacterEntity> customizer = null)
        {
            var user = GetLast<UserEntity>();
            var originEntity = GetLast<OriginEntity>();
            character = _defaultEntityCreator.CreateCharacter(user.Id, originEntity);
            var group = GetLastIfExists<GroupEntity>();
            if (group != null)
                character.GroupId = group.Id;
            return SaveEntity(character, customizer);
        }

        public TestDataUtil AddCharacterWithRequiredDependencies(Action<CharacterEntity> customizer = null)
        {
            AddUser(out var user);
            AddOrigin(out var origin);
            return SaveEntity(_defaultEntityCreator.CreateCharacter(user.Id, origin), customizer);
        }

        public TestDataUtil AddCharacterWithAllData(int ownerId, Action<CharacterEntity> customizer = null)
        {
            AddStat().AddStat().AddStat().AddStat();
            AddSkill().AddSkill();
            AddOrigin();
            AddJob().GetLast<JobEntity>();
            AddJob().GetLast<JobEntity>();

            if (!Contains<GroupEntity>())
                AddGroup(ownerId);

            AddSpeciality();

            var character = _defaultEntityCreator.CreateCharacter(ownerId, GetLast<OriginEntity>());

            character.Jobs = new List<CharacterJobEntity>
            {
                new CharacterJobEntity {Job = GetFromEnd<JobEntity>(0)},
                new CharacterJobEntity {Job = GetFromEnd<JobEntity>(1)},
            };

            var characterModifier1 = _defaultEntityCreator.CreateCharacterModifier(new List<CharacterModifierValueEntity>
            {
                _defaultEntityCreator.CreateCharacterModifierValue(GetFromEnd<StatEntity>(0), 1),
                _defaultEntityCreator.CreateCharacterModifierValue(GetFromEnd<StatEntity>(1), 2),
            });
            characterModifier1.DurationType = "combat";
            characterModifier1.CombatCount = 2;
            characterModifier1.CurrentCombatCount = 1;
            var characterModifier2 = _defaultEntityCreator.CreateCharacterModifier(new List<CharacterModifierValueEntity>
            {
                _defaultEntityCreator.CreateCharacterModifierValue(GetFromEnd<StatEntity>(2), 4),
                _defaultEntityCreator.CreateCharacterModifierValue(GetFromEnd<StatEntity>(3), 6),
            });
            var characterModifier3 = _defaultEntityCreator.CreateCharacterModifier(new List<CharacterModifierValueEntity>());
            characterModifier3.DurationType = "lap";
            characterModifier3.LapCount = 4;
            characterModifier3.CurrentLapCount = 2;
            characterModifier3.LapCountDecrement = @"{""when"":""BEFORE"", ""fighterId"": 1, ""fighterIsMonster"": true}";

            character.Modifiers = new List<CharacterModifierEntity>
            {
                characterModifier1,
                characterModifier2,
                characterModifier3,
            };

            character.Specialities = new List<CharacterSpecialityEntity>
            {
                new CharacterSpecialityEntity {Speciality = GetLast<SpecialityEntity>()}
            };

            character.Skills = new List<CharacterSkillEntity>
            {
                new CharacterSkillEntity {Skill = GetFromEnd<SkillEntity>(0)},
                new CharacterSkillEntity {Skill = GetFromEnd<SkillEntity>(1)}
            };

            character.Group = GetLast<GroupEntity>();

            return SaveEntity(character, customizer);
        }

        public TestDataUtil AddCharacterHistoryEntry(Action<CharacterHistoryEntryEntity> customizer = null)
        {
            return AddCharacterHistoryEntry(out _, customizer);
        }

        public TestDataUtil AddCharacterHistoryEntry(out CharacterHistoryEntryEntity characterHistoryEntry, Action<CharacterHistoryEntryEntity> customizer = null)
        {
            characterHistoryEntry = _defaultEntityCreator.CreateCharacterHistoryEntry(GetLast<CharacterEntity>());
            return SaveEntity(characterHistoryEntry, customizer);
        }

        public TestDataUtil AddCharacterModifier(Action<CharacterModifierEntity> customizer = null)
        {
            var characterModifier = _defaultEntityCreator.CreateCharacterModifier(new List<CharacterModifierValueEntity>());
            characterModifier.Character = GetLast<CharacterEntity>();
            return SaveEntity(characterModifier, customizer);
        }

        public TestDataUtil AddCharacterJob(Action<CharacterJobEntity> customizer = null)
        {
            return AddCharacterJob(out var _, customizer);
        }

        public TestDataUtil AddCharacterJob(out CharacterJobEntity characterJob, Action<CharacterJobEntity> customizer = null)
        {
            characterJob = new CharacterJobEntity
            {
                JobId = GetLast<JobEntity>().Id,
                CharacterId = GetLast<CharacterEntity>().Id,
            };
            return SaveEntity(characterJob, customizer);
        }
    }
}