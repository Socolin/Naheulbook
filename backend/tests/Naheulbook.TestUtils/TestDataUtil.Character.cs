using System;
using System.Collections.Generic;
using Naheulbook.Data.Models;

namespace Naheulbook.TestUtils;

public partial class TestDataUtil
{
    public TestDataUtil AddCharacter(Action<CharacterEntity> customizer = null)
    {
        return AddCharacter(out _, customizer);
    }

    public TestDataUtil AddCharacter(out CharacterEntity character, Action<CharacterEntity> customizer = null)
    {
        var user = GetLast<UserEntity>();
        var origin = GetLast<OriginEntity>();
        character = new CharacterEntity
        {
            Name = RngUtil.GetRandomString("some-character-name"),
            Sex = "Homme",
            IsActive = true,
            IsNpc = false,

            Ad = 3,
            Cha = 4,
            Cou = 5,
            Fo = 6,
            Int = 7,

            Color = "DADADA",

            Notes = "some-notes",

            Level = 1,
            Experience = 42,
            FatePoint = 1,

            Ev = 12,
            Ea = 8,

            StatBonusAd = "PRD",

            OriginId = origin.Id,
            OwnerId = user.Id,

            Jobs = new List<CharacterJobEntity>(),
            Modifiers = new List<CharacterModifierEntity>(),
            Skills = new List<CharacterSkillEntity>(),
            Specialities = new List<CharacterSpecialityEntity>(),
            Items = new List<ItemEntity>(),
            Invites = new List<GroupInviteEntity>(),
            HistoryEntries = new List<CharacterHistoryEntryEntity>(),
        };

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

        return AddCharacter(out character, customizer);
    }

    public TestDataUtil AddCharacterWithAllData(Action<CharacterEntity> customizer = null)
    {
        return AddCharacterWithAllData(out _, customizer);
    }

    public TestDataUtil AddCharacterWithAllData(out CharacterEntity character,Action<CharacterEntity> customizer = null)
    {
        if (!Contains<GroupEntity>())
            AddGroup();

        return AddOrigin()
                .AddCharacter(out character)
                .AddJob(out var job1).AddCharacterJob(job1)
                .AddJob(out var job2).AddCharacterJob(job2)
                .AddCharacterModifier(cm =>
                    {
                        cm.DurationType = "combat";
                        cm.CombatCount = 2;
                        cm.CurrentCombatCount = 1;
                    }
                )
                .AddStat(out var stat1).AddCharacterModifierValue(stat1, 1)
                .AddStat(out var stat2).AddCharacterModifierValue(stat2, 2)
                .AddCharacterModifier()
                .AddStat(out var stat3).AddCharacterModifierValue(stat3, 1)
                .AddStat(out var stat4).AddCharacterModifierValue(stat4, 2)
                .AddCharacterModifier(cm =>
                    {
                        cm.DurationType = "lap";
                        cm.LapCount = 4;
                        cm.CurrentLapCount = 2;
                        cm.LapCountDecrement = /*language=json*/ """{"when": "BEFORE", "fighterId": 1, "fighterIsMonster": true}""";
                    }
                )
                .AddSpeciality().AddCharacterSpeciality()
                .AddSkill().AddCharacterSkill()
                .AddSkill().AddCharacterSkill()
            ;
    }

    public TestDataUtil AddCharacterHistoryEntry(Action<CharacterHistoryEntryEntity> customizer = null)
    {
        return AddCharacterHistoryEntry(out _, customizer);
    }

    public TestDataUtil AddCharacterHistoryEntry(out CharacterHistoryEntryEntity characterHistoryEntry, Action<CharacterHistoryEntryEntity> customizer = null)
    {
        var character = GetLast<CharacterEntity>();

        characterHistoryEntry = new CharacterHistoryEntryEntity
        {
            Data = "{}",
            Gm = false,
            Date = new DateTime(2019, 10, 5, 5, 7, 8, DateTimeKind.Utc),
            CharacterId = character.Id,
            Action = RngUtil.GetRandomString("some-character-history-action"),
            Info = RngUtil.GetRandomString("some-info"),
        };

        return SaveEntity(characterHistoryEntry, customizer);
    }

    public TestDataUtil AddCharacterModifier(Action<CharacterModifierEntity> customizer = null)
    {
        var characterModifier = new CharacterModifierEntity
        {
            Name = RngUtil.GetRandomString("some-character-name"),
            CharacterId = GetLast<CharacterEntity>().Id,
            Description = "some-description",
            DurationType = "forever",
            IsActive = true,
            Reusable = false,
            Permanent = false,
            Values = [],
        };

        return SaveEntity(characterModifier, customizer);
    }

    public TestDataUtil AddCharacterSpeciality(Action<CharacterSpecialityEntity> customizer = null)
    {
        return AddCharacterSpeciality(out _, customizer);
    }

    public TestDataUtil AddCharacterSpeciality(out CharacterSpecialityEntity characterSpeciality, Action<CharacterSpecialityEntity> customizer = null)
    {
        var character = GetLast<CharacterEntity>();
        var speciality = GetLast<SpecialityEntity>();
        characterSpeciality = new CharacterSpecialityEntity
        {
            CharacterId = character.Id,
            SpecialityId = speciality.Id,
        };
        return SaveEntity(characterSpeciality, customizer);
    }

    public TestDataUtil AddCharacterSkill(Action<CharacterSkillEntity> customizer = null)
    {
        return AddCharacterSkill(out _, customizer);
    }

    public TestDataUtil AddCharacterSkill(out CharacterSkillEntity characterSkill, Action<CharacterSkillEntity> customizer = null)
    {
        var character = GetLast<CharacterEntity>();
        var speciality = GetLast<SkillEntity>();
        characterSkill = new CharacterSkillEntity
        {
            CharacterId = character.Id,
            SkillId = speciality.Id,
        };
        return SaveEntity(characterSkill, customizer);
    }

    public TestDataUtil AddCharacterModifierValue(
        StatEntity stat,
        short value,
        Action<CharacterModifierValueEntity> customizer = null
    )
    {
        return AddCharacterModifierValue(out _, stat, value, customizer);
    }

    public TestDataUtil AddCharacterModifierValue(
        out CharacterModifierValueEntity characterModifierValue,
        StatEntity stat,
        short value,
        Action<CharacterModifierValueEntity> customizer = null
    )
    {
        var characterModifier = GetLast<CharacterModifierEntity>();
        characterModifierValue = new CharacterModifierValueEntity
        {
            CharacterModifierId = characterModifier.Id,
            StatName = stat.Name,
            Value = value,
            Type = "ADD",
        };
        return SaveEntity(characterModifierValue, customizer);
    }

    public TestDataUtil AddCharacterJob(Action<CharacterJobEntity> customizer = null)
    {
        return AddCharacterJob(out var _, customizer);
    }

    public TestDataUtil AddCharacterJob(JobEntity job, Action<CharacterJobEntity> customizer = null)
    {
        return AddCharacterJob(out var _, job, customizer);
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