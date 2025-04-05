using System;
using FluentAssertions;
using FluentAssertions.Execution;
using Naheulbook.Core.Features.Character;
using Naheulbook.Data.EntityFrameworkCore.Entities;
using NSubstitute;
using NUnit.Framework;

namespace Naheulbook.Core.Tests.Unit.Features.Character;

public class CharacterModifierUtilTests
{
    private ICharacterHistoryUtil _characterHistoryUtil;
    private CharacterModifierUtil _util;

    [SetUp]
    public void SetUp()
    {
        _characterHistoryUtil = Substitute.For<ICharacterHistoryUtil>();

        _util = new CharacterModifierUtil(_characterHistoryUtil);
    }

    [Test]
    [TestCase(true, false)]
    [TestCase(false, true)]
    public void ToggleModifier_ShouldToggleActiveProperty(bool initialValue, bool expectedValue)
    {
        var characterModifier = new CharacterModifierEntity
        {
            IsActive = initialValue,
            Reusable = true,
        };

        _util.ToggleModifier(new CharacterEntity(), characterModifier);

        characterModifier.IsActive.Should().Be(expectedValue);
    }

    [Test]
    public void ToggleModifier_ShouldResetCurrentValuesWhenModifierIsActivated()
    {
        var characterModifier = new CharacterModifierEntity
        {
            IsActive = false,
            Reusable = true,
            CombatCount = 2,
            LapCount = 3,
            TimeDuration = 8,
        };

        _util.ToggleModifier(new CharacterEntity(), characterModifier);

        using (new AssertionScope())
        {
            characterModifier.CurrentCombatCount.Should().Be(2);
            characterModifier.CurrentLapCount.Should().Be(3);
            characterModifier.CurrentTimeDuration.Should().Be(8);
        }
    }

    [Test]
    public void ToggleModifier_ShouldLogInCharacterHistory_WhenActivated()
    {
        const int characterId = 8;
        const int characterModifierId = 12;
        var character = new CharacterEntity {Id = characterId};
        var characterHistoryEntry = new CharacterHistoryEntryEntity();
        var characterModifier = new CharacterModifierEntity
        {
            Id = characterModifierId,
            IsActive = false,
            Reusable = true,
        };

        _characterHistoryUtil.CreateLogActiveModifier(characterId, characterModifierId)
            .Returns(characterHistoryEntry);

        _util.ToggleModifier(character, characterModifier);

        character.HistoryEntries.Should().Contain(characterHistoryEntry);
    }

    [Test]
    public void ToggleModifier_ShouldLogInCharacterHistory_WhenDisabled()
    {
        const int characterId = 8;
        const int characterModifierId = 12;
        var character = new CharacterEntity {Id = characterId};
        var characterHistoryEntry = new CharacterHistoryEntryEntity();
        var characterModifier = new CharacterModifierEntity
        {
            Id = characterModifierId,
            IsActive = true,
            Reusable = true,
        };

        _characterHistoryUtil.CreateLogDisableModifier(characterId, characterModifierId)
            .Returns(characterHistoryEntry);

        _util.ToggleModifier(character, characterModifier);

        character.HistoryEntries.Should().Contain(characterHistoryEntry);
    }

    [Test]
    public void ToggleModifier_ShouldThrowIfModifierIsNotReusable()
    {
        Action act = () => _util.ToggleModifier(new CharacterEntity(), new CharacterModifierEntity {Reusable = false});

        act.Should().Throw<CharacterModifierNotReusableException>();
    }
}