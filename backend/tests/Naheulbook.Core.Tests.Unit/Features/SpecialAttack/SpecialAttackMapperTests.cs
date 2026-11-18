using AwesomeAssertions;
using Naheulbook.Core.Features.SpecialAttack;
using Naheulbook.Data.EntityFrameworkCore.Entities;
using Naheulbook.Responses.Responses;
using Naheulbook.Shared.TransientModels;
using NUnit.Framework;

namespace Naheulbook.Core.Tests.Unit.Features.SpecialAttack;

[TestFixture]
[TestOf(typeof(SpecialAttackMapper))]
public class SpecialAttackMapperTests
{
    private static readonly Guid PrerequisiteSpecialAttackId = Guid.Parse("f3a1c0de-6b0a-4c55-9a8e-1f2b3c4d5e6f");
    private SpecialAttackMapper _mapper;

    [SetUp]
    public void SetUp()
    {
        _mapper = new SpecialAttackMapper();
    }

    [Test]
    public void ToResponse_SpecialAttack_MapsAllFields()
    {
        var specialAttack = CreateSpecialAttack("some-name", 1);

        var actual = _mapper.ToResponse(specialAttack);

        actual.Should().BeEquivalentTo(new SpecialAttackResponse
        {
            Id = specialAttack.Id,
            Name = "some-name",
            Data = new SpecialAttackData
            {
                RequiredLevel = 3,
                UsableBy = "some-usable-by",
                Learning = "some-learning",
                Usage = "some-usage",
                Effects = "some-effects",
                Test = "some-test",
                Conditions = "some-conditions",
                Damage = "some-damage",
                CriticalFailure = "some-critical-failure",
                CriticalSuccess = "some-critical-success",
                CriticalNotes = "some-critical-notes",
                Range = "some-range",
                Xp = 4,
                Cooldown = new Durable { DurationType = "time", TimeDuration = 3600 },
                FailureCooldown = new Durable { DurationType = "combat", CombatCount = 1 },
                UsesPerCooldown = 2,
                Prerequisite = new SpecialAttackPrerequisite
                {
                    SpecialAttackId = PrerequisiteSpecialAttackId,
                    SuccessCount = 5,
                },
            },
        });
    }

    [Test]
    public void ToResponse_SpecialAttackGroup_MapsFieldsAndSpecialAttacksOrderedByDisplayOrder()
    {
        var specialAttack2 = CreateSpecialAttack("some-name-2", 2);
        var specialAttack1 = CreateSpecialAttack("some-name-1", 1);
        var specialAttackGroup = new SpecialAttackGroupEntity
        {
            Id = Guid.NewGuid(),
            Name = "some-group-name",
            Description = "some-group-description",
            SpecialAttacks = [specialAttack2, specialAttack1],
        };

        var actual = _mapper.ToResponse(specialAttackGroup);

        actual.Id.Should().Be(specialAttackGroup.Id);
        actual.Name.Should().Be("some-group-name");
        actual.Description.Should().Be("some-group-description");
        actual.SpecialAttacks.Select(x => x.Name).Should().Equal("some-name-1", "some-name-2");
    }

    private static SpecialAttackEntity CreateSpecialAttack(string name, int displayOrder)
    {
        return new SpecialAttackEntity
        {
            Id = Guid.NewGuid(),
            DisplayOrder = displayOrder,
            Name = name,
            Data = """
                   {
                     "requiredLevel": 3,
                     "usableBy": "some-usable-by",
                     "learning": "some-learning",
                     "usage": "some-usage",
                     "effects": "some-effects",
                     "test": "some-test",
                     "conditions": "some-conditions",
                     "damage": "some-damage",
                     "criticalFailure": "some-critical-failure",
                     "criticalSuccess": "some-critical-success",
                     "criticalNotes": "some-critical-notes",
                     "range": "some-range",
                     "xp": 4,
                     "cooldown": {"durationType": "time", "timeDuration": 3600},
                     "failureCooldown": {"durationType": "combat", "combatCount": 1},
                     "usesPerCooldown": 2,
                     "prerequisite": {"specialAttackId": "f3a1c0de-6b0a-4c55-9a8e-1f2b3c4d5e6f", "successCount": 5}
                   }
                   """,
        };
    }
}
