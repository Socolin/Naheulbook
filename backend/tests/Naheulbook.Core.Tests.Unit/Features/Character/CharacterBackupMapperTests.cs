using AwesomeAssertions;
using Naheulbook.Core.Features.Character;
using Naheulbook.Core.Features.Character.Backup.V1;
using Naheulbook.Data.EntityFrameworkCore.Entities;
using NUnit.Framework;

namespace Naheulbook.Core.Tests.Unit.Features.Character;

[TestFixture]
[TestOf(typeof(CharacterBackupMapper))]
public class CharacterBackupMapperTests
{
    private CharacterBackupMapper _mapper;

    [SetUp]
    public void SetUp()
    {
        _mapper = new CharacterBackupMapper();
    }

    [Test]
    public void ToBackup_Character_MapsAllFields()
    {
        var jobId = Guid.NewGuid();
        var skillId = Guid.NewGuid();
        var specialityId = Guid.NewGuid();
        var modifier = CreateModifier();
        var item = CreateItem();
        var character = new CharacterEntity
        {
            Id = 3,
            Name = "some-name",
            Sex = "some-sex",
            Ev = 4,
            Ea = 5,
            FatePoint = 6,
            StatBonusAd = "some-stat-bonus-ad",
            Notes = "some-notes",
            Level = 7,
            Experience = 8,
            OriginId = Guid.NewGuid(),
            Ad = 9,
            Cou = 10,
            Cha = 11,
            Fo = 12,
            Int = 13,
            Jobs = [new CharacterJobEntity {JobId = jobId}],
            Skills = [new CharacterSkillEntity {SkillId = skillId}],
            Specialities = [new CharacterSpecialityEntity {SpecialityId = specialityId}],
            Modifiers = [modifier],
            Items = [item],
        };

        var actual = _mapper.ToBackup(character);

        actual.Should().BeEquivalentTo(new BackupCharacter
        {
            Version = 1,
            Name = "some-name",
            Sex = "some-sex",
            Ev = 4,
            Ea = 5,
            FatePoint = 6,
            StatBonusAd = "some-stat-bonus-ad",
            Notes = "some-notes",
            Level = 7,
            Experience = 8,
            OriginId = character.OriginId,
            Stats = new BackupCharacter.BaseStats {Ad = 9, Cou = 10, Cha = 11, Fo = 12, Int = 13},
            JobIds = [jobId],
            SkillIds = [skillId],
            SpecialitiesIds = [specialityId],
            Modifiers = [_mapper.ToBackup(modifier)],
            Items = [_mapper.ToBackup(item)],
        });
    }

    [Test]
    public void ToBackup_CharacterModifier_MapsAllFields()
    {
        var modifier = CreateModifier();

        var actual = _mapper.ToBackup(modifier);

        actual.Should().BeEquivalentTo(new BackupCharacter.CharacterModifier
        {
            Name = "some-name",
            IsActive = true,
            Description = "some-description",
            Type = "some-type",
            Permanent = true,
            Reusable = true,
            DurationType = "combat",
            Duration = "some-duration",
            TimeDuration = 1,
            CombatCount = 2,
            LapCount = 3,
            LapCountDecrement = "some-lap-count-decrement",
            CurrentCombatCount = 4,
            CurrentTimeDuration = 5,
            CurrentLapCount = 6,
            Values = [new BackupCharacter.CharacterModifierValue {StatName = "AD", Type = "ADD", Value = 2}],
        });
    }

    [Test]
    public void ToBackup_Item_MapsFieldsAndItemTemplate()
    {
        var item = CreateItem();

        var actual = _mapper.ToBackup(item);

        actual.Should().BeEquivalentTo(new BackupCharacterItem
        {
            Data = "some-data",
            Modifiers = "some-modifiers",
            ContainerId = 3,
            ItemTemplate = _mapper.ToBackup(item.ItemTemplate),
        });
    }

    [Test]
    public void ToBackup_ItemTemplate_MapsAllFields()
    {
        var skillId = Guid.NewGuid();
        var unSkillId = Guid.NewGuid();
        var skillModifierSkillId = Guid.NewGuid();
        var jobId = Guid.NewGuid();
        var originId = Guid.NewGuid();
        var itemTemplate = new ItemTemplateEntity
        {
            Id = Guid.NewGuid(),
            Name = "some-name",
            TechName = "some-tech-name",
            SourceUserNameCache = "some-source-user-name",
            Data = "some-data",
            SubCategoryId = 3,
            Modifiers = [new ItemTemplateModifierEntity {Value = 1, Special = "a,b", Type = "ADD", RequiredJobId = jobId, RequiredOriginId = originId, StatName = "AD"}],
            Requirements = [new ItemTemplateRequirementEntity {MinValue = 2, MaxValue = 3, StatName = "FO"}],
            SkillModifiers = [new ItemTemplateSkillModifierEntity {SkillId = skillModifierSkillId, Value = 4}],
            Slots = [new ItemTemplateSlotEntity {Slot = new SlotEntity {TechName = "SOME_SLOT"}}],
            Skills = [new ItemTemplateSkillEntity {SkillId = skillId}],
            UnSkills = [new ItemTemplateUnSkillEntity {SkillId = unSkillId}],
        };

        var actual = _mapper.ToBackup(itemTemplate);

        actual.Should().BeEquivalentTo(new BackupItemTemplate
        {
            Id = itemTemplate.Id,
            Name = "some-name",
            TechName = "some-tech-name",
            SourceUserNameCache = "some-source-user-name",
            Data = "some-data",
            SubCategoryId = 3,
            Modifiers = [new BackupItemTemplate.BackupItemTemplateModifier {Value = 1, Special = "a,b", Type = "ADD", RequiredJobId = jobId, RequiredOriginId = originId, StatName = "AD"}],
            Requirements = [new BackupItemTemplate.BackupItemTemplateRequirement {MinValue = 2, MaxValue = 3, StatName = "FO"}],
            SkillModifiers = [new BackupItemTemplate.BackupItemTemplateSkillModifier {SkillId = skillModifierSkillId, Value = 4}],
            Slots = ["SOME_SLOT"],
            Skills = [skillId],
            UnSkills = [unSkillId],
        });
    }

    private static CharacterModifierEntity CreateModifier()
    {
        return new CharacterModifierEntity
        {
            Id = 42,
            Name = "some-name",
            IsActive = true,
            Description = "some-description",
            Type = "some-type",
            Permanent = true,
            Reusable = true,
            DurationType = "combat",
            Duration = "some-duration",
            TimeDuration = 1,
            CombatCount = 2,
            LapCount = 3,
            LapCountDecrement = "some-lap-count-decrement",
            CurrentCombatCount = 4,
            CurrentTimeDuration = 5,
            CurrentLapCount = 6,
            Values = [new CharacterModifierValueEntity {Id = 7, StatName = "AD", Type = "ADD", Value = 2}],
        };
    }

    private static ItemEntity CreateItem()
    {
        return new ItemEntity
        {
            Id = 42,
            Data = "some-data",
            Modifiers = "some-modifiers",
            ContainerId = 3,
            ItemTemplate = new ItemTemplateEntity
            {
                Name = "some-template-name",
                Modifiers = [],
                Requirements = [],
                SkillModifiers = [],
                Slots = [],
                Skills = [],
                UnSkills = [],
            },
        };
    }
}
