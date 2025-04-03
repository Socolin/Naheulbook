using System;
using System.Collections.Generic;
using Naheulbook.Data.Models;

namespace Naheulbook.TestUtils;

public partial class TestDataUtil
{
    public TestDataUtil AddCharacter(int ownerId, Action<CharacterEntity> customizer = null)
    {
        return SaveEntity(defaultEntityCreator.CreateCharacter(ownerId, GetLast<OriginEntity>()), customizer);
    }

    public TestDataUtil AddCharacter(Action<CharacterEntity> customizer = null)
    {
        return AddCharacter(out _, customizer);
    }

    public TestDataUtil AddCharacter(out CharacterEntity character, Action<CharacterEntity> customizer = null)
    {
        var user = GetLast<UserEntity>();
        var originEntity = GetLast<OriginEntity>();
        character = defaultEntityCreator.CreateCharacter(user.Id, originEntity);
        var group = GetLastIfExists<GroupEntity>();
        if (group != null)
            character.GroupId = group.Id;
        return SaveEntity(character, customizer);
    }

    public TestDataUtil AddCharacterWithRequiredDependencies(Action<CharacterEntity> customizer = null)
    {
        return AddCharacterWithRequiredDependencies(out _, customizer);
    }

    public TestDataUtil AddCharacterWithRequiredDependencies(out CharacterEntity character, Action<CharacterEntity> customizer = null)
    {
        var user = GetLastIfExists<UserEntity>();
        if (user is null)
            AddUser(out user);

        var origin = GetLastIfExists<OriginEntity>();
        if (origin is null)
            AddOrigin(out origin);

        character = defaultEntityCreator.CreateCharacter(user.Id, origin);

        return SaveEntity(character, customizer);
    }

    public TestDataUtil AddCharacterWithAllData(int ownerId, Action<CharacterEntity> customizer = null)
    {
        return AddCharacterWithAllData(out _, ownerId, customizer);
    }

    public TestDataUtil AddCharacterWithAllData(out CharacterEntity character, int ownerId, Action<CharacterEntity> customizer = null)
    {
        AddStat().AddStat().AddStat().AddStat();
        AddSkill().AddSkill();
        AddOrigin();

        AddJob(out var job1);
        AddJob(out var job2);

        if (!Contains<GroupEntity>())
            AddGroup(ownerId);

        AddSpeciality();

        character = defaultEntityCreator.CreateCharacter(ownerId, GetLast<OriginEntity>());

        character.Jobs = new List<CharacterJobEntity>
        {
            new() {Job = job2},
            new() {Job = job1},
        };

        var characterModifier1 = defaultEntityCreator.CreateCharacterModifier(new List<CharacterModifierValueEntity>
            {
                defaultEntityCreator.CreateCharacterModifierValue(GetFromEnd<StatEntity>(0), 1),
                defaultEntityCreator.CreateCharacterModifierValue(GetFromEnd<StatEntity>(1), 2),
            }
        );
        characterModifier1.DurationType = "combat";
        characterModifier1.CombatCount = 2;
        characterModifier1.CurrentCombatCount = 1;
        var characterModifier2 = defaultEntityCreator.CreateCharacterModifier(new List<CharacterModifierValueEntity>
            {
                defaultEntityCreator.CreateCharacterModifierValue(GetFromEnd<StatEntity>(2), 4),
                defaultEntityCreator.CreateCharacterModifierValue(GetFromEnd<StatEntity>(3), 6),
            }
        );
        var characterModifier3 = defaultEntityCreator.CreateCharacterModifier(new List<CharacterModifierValueEntity>());
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
            new() {Speciality = GetLast<SpecialityEntity>()},
        };

        character.Skills = new List<CharacterSkillEntity>
        {
            new() {Skill = GetFromEnd<SkillEntity>(0)},
            new() {Skill = GetFromEnd<SkillEntity>(1)},
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
        characterHistoryEntry = defaultEntityCreator.CreateCharacterHistoryEntry(GetLast<CharacterEntity>());
        return SaveEntity(characterHistoryEntry, customizer);
    }

    public TestDataUtil AddCharacterModifier(Action<CharacterModifierEntity> customizer = null)
    {
        var characterModifier = defaultEntityCreator.CreateCharacterModifier(new List<CharacterModifierValueEntity>());
        characterModifier.Character = GetLast<CharacterEntity>();
        return SaveEntity(characterModifier, customizer);
    }

    public TestDataUtil AddCharacterJob(Action<CharacterJobEntity> customizer = null)
    {
        return AddCharacterJob(out var _, customizer);
    }

    public TestDataUtil AddCharacterJob(out CharacterJobEntity characterJob, Action<CharacterJobEntity> customizer = null)
    {
        return AddCharacterJob(out characterJob, GetLast<JobEntity>(), customizer);
    }

    public TestDataUtil AddCharacterJob(out CharacterJobEntity characterJob, JobEntity job, Action<CharacterJobEntity> customizer = null)
    {
        characterJob = new CharacterJobEntity
        {
            JobId = job.Id,
            CharacterId = GetLast<CharacterEntity>().Id,
        };
        return SaveEntity(characterJob, customizer);
    }
}