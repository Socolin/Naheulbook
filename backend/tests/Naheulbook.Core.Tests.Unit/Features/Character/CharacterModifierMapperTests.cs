using AwesomeAssertions;
using Naheulbook.Core.Features.Character;
using Naheulbook.Data.EntityFrameworkCore.Entities;
using Naheulbook.Requests.Requests;
using Naheulbook.Shared.TransientModels;
using NUnit.Framework;

namespace Naheulbook.Core.Tests.Unit.Features.Character;

[TestFixture]
[TestOf(typeof(CharacterModifierMapper))]
public class CharacterModifierMapperTests
{
    private CharacterModifierMapper _mapper;

    [SetUp]
    public void SetUp()
    {
        _mapper = new CharacterModifierMapper();
    }

    [Test]
    public void ToResponse_CharacterModifier_MapsAllFieldsAndOrdersValuesById()
    {
        var value1 = new CharacterModifierValueEntity {Id = 1, StatName = "AD", Type = "ADD", Value = 1};
        var value2 = new CharacterModifierValueEntity {Id = 2, StatName = "FO", Type = "ADD", Value = 2};
        var characterModifier = new CharacterModifierEntity
        {
            Id = 3,
            Name = "some-name",
            IsActive = true,
            Permanent = true,
            Reusable = true,
            Description = "some-description",
            Type = "some-type",
            DurationType = "combat",
            Duration = "some-duration",
            CombatCount = 4,
            LapCount = 5,
            TimeDuration = 6,
            CurrentCombatCount = 7,
            CurrentLapCount = 8,
            CurrentTimeDuration = 9,
            LapCountDecrement = """{"when":"BEFORE","fighterId":10,"fighterIsMonster":true}""",
            Values = [value2, value1],
        };

        var actual = _mapper.ToResponse(characterModifier);

        actual.Should().BeEquivalentTo(new ActiveStatsModifier
        {
            Id = 3,
            Name = "some-name",
            Active = true,
            Permanent = true,
            Reusable = true,
            Description = "some-description",
            Type = "some-type",
            DurationType = "combat",
            Duration = "some-duration",
            CombatCount = 4,
            LapCount = 5,
            TimeDuration = 6,
            CurrentCombatCount = 7,
            CurrentLapCount = 8,
            CurrentTimeDuration = 9,
            LapCountDecrement = new LapCountDecrement {When = "BEFORE", FighterId = 10, FighterIsMonster = true},
            Values = [_mapper.ToResponse(value1), _mapper.ToResponse(value2)],
        }, o => o.WithStrictOrdering());
    }

    [Test]
    public void ToResponse_CharacterModifierWithoutLapCountDecrement_ReturnsNullLapCountDecrement()
    {
        var characterModifier = new CharacterModifierEntity
        {
            Name = "some-name",
            DurationType = "forever",
            LapCountDecrement = null,
            Values = [],
        };

        var actual = _mapper.ToResponse(characterModifier);

        actual.LapCountDecrement.Should().BeNull();
    }

    [Test]
    public void ToResponse_CharacterModifierValue_MapsStatNameToStat()
    {
        var value = new CharacterModifierValueEntity {Id = 1, CharacterModifierId = 2, StatName = "CHA", Type = "ADD", Value = -2};

        var actual = _mapper.ToResponse(value);

        actual.Should().BeEquivalentTo(new StatModifierRequest
        {
            Stat = "CHA",
            Type = "ADD",
            Value = -2,
            Special = null,
        });
    }

    [Test]
    public void ToEntity_AddCharacterModifierRequest_MapsFieldsAndInitializeCurrentValues()
    {
        var request = new AddCharacterModifierRequest
        {
            Name = "some-name",
            Reusable = true,
            Description = "some-description",
            Type = "some-type",
            DurationType = "combat",
            Duration = "some-duration",
            CombatCount = 4,
            LapCount = 5,
            TimeDuration = 6,
            LapCountDecrement = new LapCountDecrement {When = "BEFORE", FighterId = 10, FighterIsMonster = true},
            Values = [new StatModifierRequest {Stat = "AD", Type = "ADD", Value = 2}],
        };

        var actual = _mapper.ToEntity(request);

        actual.Should().BeEquivalentTo(new CharacterModifierEntity
        {
            Name = "some-name",
            IsActive = true,
            Permanent = false,
            Reusable = true,
            Description = "some-description",
            Type = "some-type",
            DurationType = "combat",
            Duration = "some-duration",
            CombatCount = 4,
            LapCount = 5,
            TimeDuration = 6,
            CurrentCombatCount = 4,
            CurrentLapCount = 5,
            CurrentTimeDuration = 6,
            LapCountDecrement = """{"When":"BEFORE","FighterId":10,"FighterIsMonster":true}""",
            Values = [new CharacterModifierValueEntity {StatName = "AD", Type = "ADD", Value = 2}],
        });
    }
}
