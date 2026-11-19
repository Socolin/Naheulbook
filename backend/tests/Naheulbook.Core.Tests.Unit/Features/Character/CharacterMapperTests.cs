using AwesomeAssertions;
using Naheulbook.Core.Features.Aptitude;
using Naheulbook.Core.Features.Character;
using Naheulbook.Core.Features.Group;
using Naheulbook.Core.Features.Item;
using Naheulbook.Core.Features.Job;
using Naheulbook.Core.Features.Handicap;
using Naheulbook.Core.Features.Mastery;
using Naheulbook.Core.Features.SpecialAttack;
using Naheulbook.Core.Features.Talent;
using Naheulbook.Data.EntityFrameworkCore.Entities;
using Naheulbook.Responses.Responses;
using Naheulbook.Shared.TransientModels;
using Newtonsoft.Json.Linq;
using NSubstitute;
using NUnit.Framework;

namespace Naheulbook.Core.Tests.Unit.Features.Character;

[TestFixture]
[TestOf(typeof(CharacterMapper))]
public class CharacterMapperTests
{
    private IAptitudeMapper _aptitudeMapper;
    private ICharacterModifierMapper _characterModifierMapper;
    private IGroupMapper _groupMapper;
    private IItemMapper _itemMapper;
    private ISpecialityMapper _specialityMapper;
    private ISpecialAttackMapper _specialAttackMapper;
    private ITalentMapper _talentMapper;
    private IMasteryMapper _masteryMapper;
    private IHandicapMapper _handicapMapper;
    private CharacterMapper _mapper;

    [SetUp]
    public void SetUp()
    {
        _aptitudeMapper = Substitute.For<IAptitudeMapper>();
        _characterModifierMapper = Substitute.For<ICharacterModifierMapper>();
        _groupMapper = Substitute.For<IGroupMapper>();
        _itemMapper = Substitute.For<IItemMapper>();
        _specialityMapper = Substitute.For<ISpecialityMapper>();
        _specialAttackMapper = Substitute.For<ISpecialAttackMapper>();
        _talentMapper = Substitute.For<ITalentMapper>();
        _masteryMapper = Substitute.For<IMasteryMapper>();
        _handicapMapper = Substitute.For<IHandicapMapper>();
        _mapper = new CharacterMapper(_aptitudeMapper, _characterModifierMapper, _groupMapper, _itemMapper, _specialityMapper, _specialAttackMapper, _talentMapper, _masteryMapper, _handicapMapper);
    }

    [Test]
    public void ToNamedIdResponse_Character_MapsIdAndName()
    {
        var character = new CharacterEntity {Id = 3, Name = "some-name"};

        var actual = _mapper.ToNamedIdResponse(character);

        actual.Should().BeEquivalentTo(new NamedIdResponse {Id = 3, Name = "some-name"});
    }

    [Test]
    public void ToCreateResponse_Character_MapsId()
    {
        var character = new CharacterEntity {Id = 3, Name = "some-name"};

        var actual = _mapper.ToCreateResponse(character);

        actual.Should().BeEquivalentTo(new CreateCharacterResponse {Id = 3});
    }

    [Test]
    public void ToListActiveResponse_Character_MapsIdNameAndIsNpc()
    {
        var character = new CharacterEntity {Id = 3, Name = "some-name", IsNpc = true};

        var actual = _mapper.ToListActiveResponse(character);

        actual.Should().BeEquivalentTo(new ListActiveCharacterResponse {Id = 3, Name = "some-name", IsNpc = true});
    }

    [Test]
    public void ToSearchResponse_Character_MapsOriginAndOwnerNames()
    {
        var character = new CharacterEntity
        {
            Id = 3,
            Name = "some-name",
            Origin = new OriginEntity {Name = "some-origin-name"},
            Owner = new UserEntity {DisplayName = "some-owner-name"},
        };

        var actual = _mapper.ToSearchResponse(character);

        actual.Should().BeEquivalentTo(new CharacterSearchResponse
        {
            Id = 3,
            Name = "some-name",
            OriginName = "some-origin-name",
            OwnerName = "some-owner-name",
        });
    }

    [Test]
    public void ToSummaryResponse_Character_MapsLevelOriginNameAndJobNames()
    {
        var character = new CharacterEntity
        {
            Id = 3,
            Name = "some-name",
            Level = 4,
            Origin = new OriginEntity {Name = "some-origin-name"},
            Jobs =
            [
                new CharacterJobEntity {Job = new JobEntity {Name = "some-job-1"}},
                new CharacterJobEntity {Job = new JobEntity {Name = "some-job-2"}},
            ],
        };

        var actual = _mapper.ToSummaryResponse(character);

        actual.Should().BeEquivalentTo(new CharacterSummaryResponse
        {
            Id = 3,
            Name = "some-name",
            Level = 4,
            OriginName = "some-origin-name",
            JobNames = ["some-job-1", "some-job-2"],
        }, o => o.WithStrictOrdering());
    }

    [Test]
    public void ToResponse_Character_MapsAllFields()
    {
        var setup = new CharacterSetup(this);

        var actual = _mapper.ToResponse(setup.Character);

        actual.Should().BeOfType<CharacterResponse>();
        actual.Should().BeEquivalentTo(setup.ExpectedResponse<CharacterResponse>());
    }

    [Test]
    public void ToResponse_CharacterWithoutGroup_ReturnsNullGroup()
    {
        var setup = new CharacterSetup(this);
        setup.Character.GroupId = null;
        setup.Character.Group = null;

        var actual = _mapper.ToResponse(setup.Character);

        actual.Group.Should().BeNull();
    }

    [Test]
    public void ToGmResponse_Character_MapsAllFieldsIncludingGmOnlyFields()
    {
        var setup = new CharacterSetup(this);
        setup.Character.IsActive = true;
        setup.Character.Color = "some-color";
        setup.Character.GmData = """{"mankdebol":3,"debilibeuk":4}""";
        setup.Character.OwnerId = 12;
        setup.Character.TargetedCharacterId = 13;

        var actual = _mapper.ToGmResponse(setup.Character);

        var expected = setup.ExpectedResponse<CharacterFoGmResponse>();
        expected.IsActive = true;
        expected.Color = "some-color";
        expected.GmData = new CharacterGmData {Mankdebol = 3, Debilibeuk = 4};
        expected.OwnerId = 12;
        expected.Target = new TargetResponse {Id = 13, IsMonster = false};
        actual.Should().BeEquivalentTo(expected);
    }

    [Test]
    public void ToGmResponse_CharacterWithoutGmData_ReturnsDefaultGmData()
    {
        var setup = new CharacterSetup(this);
        setup.Character.GmData = null;

        var actual = _mapper.ToGmResponse(setup.Character);

        actual.GmData.Should().BeEquivalentTo(new CharacterGmData());
    }

    [Test]
    public void ToGmResponse_CharacterTargetingMonster_ReturnsMonsterTarget()
    {
        var setup = new CharacterSetup(this);
        setup.Character.TargetedMonsterId = 14;

        var actual = _mapper.ToGmResponse(setup.Character);

        actual.Target.Should().BeEquivalentTo(new TargetResponse {Id = 14, IsMonster = true});
    }

    [Test]
    public void ToGmResponse_CharacterWithoutTarget_ReturnsNullTarget()
    {
        var setup = new CharacterSetup(this);

        var actual = _mapper.ToGmResponse(setup.Character);

        actual.Target.Should().BeNull();
    }

    [Test]
    public void ToResponse_LevelUpResult_MapsNewModifiersSkillsSpecialitiesTalentsAndMasteries()
    {
        var talent = new TalentEntity();
        var talentResponse = new TalentResponse();
        var mastery = new MasteryEntity();
        var masteryResponse = new MasteryResponse();
        var modifier = new CharacterModifierEntity();
        var modifierResponse = new ActiveStatsModifier();
        var speciality = new SpecialityEntity();
        var specialityResponse = new SpecialityResponse();
        var skillId = Guid.NewGuid();
        var levelUpResult = new LevelUpResult
        {
            NewLevel = 3,
            NewModifiers = [modifier],
            NewSkills = [new CharacterSkillEntity {SkillId = skillId}],
            NewSpecialities = [new CharacterSpecialityEntity {Speciality = speciality}],
            NewTalents = [talent],
            NewMasteries = [mastery],
        };

        _characterModifierMapper.ToResponse(modifier)
            .Returns(modifierResponse);
        _specialityMapper.ToResponse(speciality)
            .Returns(specialityResponse);
        _talentMapper.ToResponse(talent)
            .Returns(talentResponse);
        _masteryMapper.ToResponse(mastery)
            .Returns(masteryResponse);

        var actual = _mapper.ToResponse(levelUpResult);

        actual.Should().BeEquivalentTo(new CharacterLevelUpResponse
        {
            NewLevel = 3,
            NewModifiers = [modifierResponse],
            NewSkillIds = [skillId],
            NewSpecialities = [specialityResponse],
            NewTalents = [talentResponse],
            NewMasteries = [masteryResponse],
        });
    }

    [Test]
    public void ToResponse_CharacterHistoryEntry_MapsAllFields()
    {
        var historyEntry = new CharacterHistoryEntryEntity
        {
            Id = 3,
            Action = "some-action",
            Data = """{"key":"value"}""",
            Date = new DateTime(2024, 3, 4, 5, 6, 7, DateTimeKind.Utc),
            Gm = true,
        };

        var actual = _mapper.ToResponse(historyEntry);

        actual.Should().BeEquivalentTo(new CharacterHistoryEntryResponse
        {
            Id = 3,
            Action = "some-action",
            Data = new JObject {["key"] = "value"},
            Date = "2024-03-04T05:06:07Z",
            Gm = true,
        });
    }

    [Test]
    public void ToResponse_CharacterHistoryEntryWithoutData_ReturnsEmptyData()
    {
        var historyEntry = new CharacterHistoryEntryEntity {Action = "some-action", Data = null};

        var actual = _mapper.ToResponse(historyEntry);

        actual.Data.Should().BeEquivalentTo(new JObject());
    }

    [Test]
    public void ToResponse_IHistoryEntryFromCharacter_MapsAsCharacterHistoryEntry()
    {
        IHistoryEntry historyEntry = new CharacterHistoryEntryEntity {Id = 3, Action = "some-action"};

        var actual = _mapper.ToResponse(historyEntry);

        actual.Should().BeOfType<CharacterHistoryEntryResponse>()
            .Which.Id.Should().Be(3);
    }

    [Test]
    public void ToResponse_IHistoryEntryFromGroup_UsesGroupMapper()
    {
        var groupHistoryEntry = new GroupHistoryEntryEntity();
        var groupHistoryEntryResponse = new GroupHistoryEntryResponse();

        _groupMapper.ToResponse(groupHistoryEntry)
            .Returns(groupHistoryEntryResponse);

        var actual = _mapper.ToResponse((IHistoryEntry)groupHistoryEntry);

        actual.Should().BeSameAs(groupHistoryEntryResponse);
    }

    private class CharacterSetup
    {
        public readonly CharacterEntity Character;

        private readonly Guid _jobId = Guid.NewGuid();
        private readonly Guid _skillId = Guid.NewGuid();
        private readonly CharacterGroupResponse _groupResponse = new();
        private readonly CharacterAptitudeResponse _aptitudeResponse = new();
        private readonly CharacterSpecialAttackResponse _specialAttackResponse = new();
        private readonly TalentResponse _talentResponse = new() {Name = "some-talent"};
        private readonly MasteryResponse _masteryResponse = new() {Name = "some-mastery"};
        private readonly HandicapResponse _handicapResponse = new() {DiceRoll = 4};
        private readonly ActiveStatsModifier _modifierResponse = new();
        private readonly SpecialityResponse _specialityResponse = new();
        private readonly ItemResponse _itemResponse = new();
        private readonly CharacterGroupInviteResponse _inviteResponse = new();

        public CharacterSetup(CharacterMapperTests tests)
        {
            var group = new GroupEntity();
            var aptitude = new CharacterAptitudeEntity();
            var specialAttack = new CharacterSpecialAttackEntity();
            var talent = new TalentEntity();
            var mastery = new MasteryEntity();
            var handicap = new HandicapEntity();
            var modifier = new CharacterModifierEntity();
            var speciality = new SpecialityEntity();
            var item = new ItemEntity();
            var invite = new GroupInviteEntity();

            Character = new CharacterEntity
            {
                Id = 3,
                Name = "some-name",
                Sex = "some-sex",
                OriginId = Guid.NewGuid(),
                IsNpc = true,
                Ev = 4,
                Ea = 5,
                Notes = "some-notes",
                Level = 6,
                Experience = 7,
                FatePoint = 8,
                Ad = 9,
                Cou = 10,
                Cha = 11,
                Fo = 12,
                Int = 13,
                StatBonusAd = "some-stat-bonus-ad",
                GroupId = 14,
                Group = group,
                Jobs = [new CharacterJobEntity {JobId = _jobId}],
                Skills = [new CharacterSkillEntity {SkillId = _skillId}],
                Aptitudes = [aptitude],
                SpecialAttacks = [specialAttack],
                Talents = [new CharacterTalentEntity {Talent = talent}],
                Masteries = [new CharacterMasteryEntity {Mastery = mastery}],
                Handicaps = [new CharacterHandicapEntity {Handicap = handicap}],
                Modifiers = [modifier],
                Specialities = [new CharacterSpecialityEntity {Speciality = speciality}],
                Items = [item],
                Invites = [invite],
            };

            tests._groupMapper.ToCharacterGroupResponse(group).Returns(_groupResponse);
            tests._aptitudeMapper.ToResponse(aptitude).Returns(_aptitudeResponse);
            tests._specialAttackMapper.ToResponse(specialAttack).Returns(_specialAttackResponse);
            tests._talentMapper.ToResponse(talent).Returns(_talentResponse);
            tests._masteryMapper.ToResponse(mastery).Returns(_masteryResponse);
            tests._handicapMapper.ToResponse(handicap).Returns(_handicapResponse);
            tests._characterModifierMapper.ToResponse(modifier).Returns(_modifierResponse);
            tests._specialityMapper.ToResponse(speciality).Returns(_specialityResponse);
            tests._itemMapper.ToResponse(item).Returns(_itemResponse);
            tests._groupMapper.ToCharacterInviteResponse(invite).Returns(_inviteResponse);
        }

        public T ExpectedResponse<T>()
            where T : CharacterResponse, new()
        {
            return new T
            {
                Id = 3,
                Name = "some-name",
                Sex = "some-sex",
                OriginId = Character.OriginId,
                IsNpc = true,
                Ev = 4,
                Ea = 5,
                Notes = "some-notes",
                Level = 6,
                Experience = 7,
                FatePoint = 8,
                Stats = new CharacterResponse.BasicStats {Ad = 9, Cou = 10, Cha = 11, Fo = 12, Int = 13},
                StatBonusAd = "some-stat-bonus-ad",
                JobIds = [_jobId],
                SkillIds = [_skillId],
                Group = _groupResponse,
                Aptitudes = [_aptitudeResponse],
                SpecialAttacks = [_specialAttackResponse],
                Talents = [_talentResponse],
                Masteries = [_masteryResponse],
                Handicaps = [_handicapResponse],
                Modifiers = [_modifierResponse],
                Specialities = [_specialityResponse],
                Items = [_itemResponse],
                Invites = [_inviteResponse],
            };
        }
    }
}
