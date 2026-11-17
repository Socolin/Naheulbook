using AwesomeAssertions;
using Naheulbook.Core.Features.Origin;
using Naheulbook.Data.EntityFrameworkCore.Entities;
using Naheulbook.Responses.Responses;
using Naheulbook.Shared.TransientModels;
using NUnit.Framework;

namespace Naheulbook.Core.Tests.Unit.Features.Origin;

[TestFixture]
[TestOf(typeof(OriginMapper))]
public class OriginMapperTests
{
    private OriginMapper _mapper;

    [SetUp]
    public void SetUp()
    {
        _mapper = new OriginMapper();
    }

    [Test]
    public void ToResponse_Origin_MapsAllFields()
    {
        var skillId1 = new Guid("00000000-0000-0000-0000-000000000001");
        var skillId2 = new Guid("00000000-0000-0000-0000-000000000002");
        var skillId3 = new Guid("00000000-0000-0000-0000-000000000003");
        var information = new OriginInfoEntity {Title = "some-title", Description = "some-description"};
        var bonus = new OriginBonusEntity {Description = "some-bonus"};
        var requirement1 = new OriginRequirementEntity {Id = 1, StatName = "AD"};
        var requirement2 = new OriginRequirementEntity {Id = 2, StatName = "FO"};
        var restriction = new OriginRestrictEntity {Text = "some-restriction"};
        var origin = new OriginEntity
        {
            Id = Guid.NewGuid(),
            Name = "some-name",
            Description = "some-description",
            PlayerDescription = "some-player-description",
            PlayerSummary = "some-player-summary",
            Advantage = "some-advantage",
            Size = "some-size",
            Data = """{"baseEv":20,"diceEvLevelUp":6}""",
            Flags = """[{"type":"some-flag"}]""",
            AptitudeGroupId = Guid.NewGuid(),
            Skills =
            [
                new OriginSkillEntity {SkillId = skillId3, Default = true},
                new OriginSkillEntity {SkillId = skillId2, Default = false},
                new OriginSkillEntity {SkillId = skillId1, Default = true},
            ],
            Information = [information],
            Bonuses = [bonus],
            Requirements = [requirement2, requirement1],
            Restrictions = [restriction],
        };

        var actual = _mapper.ToResponse(origin);

        actual.Should().BeEquivalentTo(new OriginResponse
        {
            Id = origin.Id,
            Name = "some-name",
            Description = "some-description",
            PlayerDescription = "some-player-description",
            PlayerSummary = "some-player-summary",
            Advantage = "some-advantage",
            Size = "some-size",
            Data = new OriginData {BaseEv = 20, DiceEvLevelUp = 6},
            Flags = [new FlagResponse {Type = "some-flag"}],
            SkillIds = [skillId1, skillId3],
            AvailableSkillIds = [skillId2],
            AptitudeGroupId = origin.AptitudeGroupId,
            Information = [_mapper.ToResponse(information)],
            Bonuses = [_mapper.ToResponse(bonus)],
            Requirements = [_mapper.ToResponse(requirement1), _mapper.ToResponse(requirement2)],
            Restrictions = [_mapper.ToResponse(restriction)],
        }, o => o.WithStrictOrdering());
    }

    [Test]
    public void ToResponse_OriginWithoutFlags_ReturnsEmptyFlags()
    {
        var origin = new OriginEntity
        {
            Name = "some-name",
            Description = "some-description",
            Data = "{}",
            Flags = null,
            Skills = [],
            Information = [],
            Bonuses = [],
            Requirements = [],
            Restrictions = [],
        };

        var actual = _mapper.ToResponse(origin);

        actual.Flags.Should().NotBeNull().And.BeEmpty();
    }

    [Test]
    public void ToResponse_OriginInfo_MapsTitleAndDescription()
    {
        var information = new OriginInfoEntity {Id = 4, Title = "some-title", Description = "some-description"};

        var actual = _mapper.ToResponse(information);

        actual.Should().BeEquivalentTo(new OriginInformationResponse
        {
            Title = "some-title",
            Description = "some-description",
        });
    }

    [Test]
    public void ToResponse_OriginRequirement_MapsStatAndMinMax()
    {
        var requirement = new OriginRequirementEntity {StatName = "CHA", MinValue = 9, MaxValue = null};

        var actual = _mapper.ToResponse(requirement);

        actual.Should().BeEquivalentTo(new OriginRequirementResponse
        {
            Stat = "CHA",
            Min = 9,
            Max = null,
        });
    }

    [Test]
    public void ToResponse_OriginBonus_MapsDescriptionAndFlags()
    {
        var bonus = new OriginBonusEntity {Description = "some-description", Flags = """[{"type":"some-flag"}]"""};

        var actual = _mapper.ToResponse(bonus);

        actual.Should().BeEquivalentTo(new DescribedFlagResponse
        {
            Description = "some-description",
            Flags = [new FlagResponse {Type = "some-flag"}],
        });
    }

    [Test]
    public void ToResponse_OriginRestrict_MapsTextToDescriptionAndFlags()
    {
        var restriction = new OriginRestrictEntity {Text = "some-text", Flags = null};

        var actual = _mapper.ToResponse(restriction);

        actual.Should().BeEquivalentTo(new DescribedFlagResponse
        {
            Description = "some-text",
            Flags = [],
        });
    }
}
