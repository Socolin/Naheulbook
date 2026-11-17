using AwesomeAssertions;
using Naheulbook.Core.Features.Job;
using Naheulbook.Data.EntityFrameworkCore.Entities;
using Naheulbook.Responses.Responses;
using NSubstitute;
using NUnit.Framework;

namespace Naheulbook.Core.Tests.Unit.Features.Job;

[TestFixture]
[TestOf(typeof(JobMapper))]
public class JobMapperTests
{
    private ISpecialityMapper _specialityMapper;
    private JobMapper _mapper;

    [SetUp]
    public void SetUp()
    {
        _specialityMapper = Substitute.For<ISpecialityMapper>();
        _mapper = new JobMapper(_specialityMapper);
    }

    [Test]
    public void ToResponse_Job_MapsAllFields()
    {
        var skillId1 = new Guid("00000000-0000-0000-0000-000000000001");
        var skillId2 = new Guid("00000000-0000-0000-0000-000000000002");
        var skillId3 = new Guid("00000000-0000-0000-0000-000000000003");
        var skillId4 = new Guid("00000000-0000-0000-0000-000000000004");
        var bonus = new JobBonusEntity {Description = "some-bonus"};
        var requirement1 = new JobRequirementEntity {Id = 1, StatName = "AD"};
        var requirement2 = new JobRequirementEntity {Id = 2, StatName = "FO"};
        var restriction = new JobRestrictionEntity {Text = "some-restriction"};
        var speciality = new SpecialityEntity();
        var specialityResponse = new SpecialityResponse();
        var job = new JobEntity
        {
            Id = Guid.NewGuid(),
            Name = "some-name",
            Information = "some-information",
            PlayerDescription = "some-player-description",
            PlayerSummary = "some-player-summary",
            IsMagic = true,
            Data = """{"forOrigin":{"all":{"diceEvLevelUp":4}}}""",
            Flags = """[{"type":"some-flag"}]""",
            Skills =
            [
                new JobSkillEntity {SkillId = skillId4, Default = true},
                new JobSkillEntity {SkillId = skillId3, Default = false},
                new JobSkillEntity {SkillId = skillId2, Default = true},
                new JobSkillEntity {SkillId = skillId1, Default = false},
            ],
            Bonuses = [bonus],
            Requirements = [requirement2, requirement1],
            Restrictions = [restriction],
            Specialities = [speciality],
        };

        _specialityMapper.ToResponse(speciality)
            .Returns(specialityResponse);

        var actual = _mapper.ToResponse(job);

        actual.Should().BeEquivalentTo(new
        {
            Id = job.Id,
            Name = "some-name",
            Information = "some-information",
            PlayerDescription = "some-player-description",
            PlayerSummary = "some-player-summary",
            IsMagic = true,
            Flags = new[] {new FlagResponse {Type = "some-flag"}},
            SkillIds = new[] {skillId2, skillId4},
            AvailableSkillIds = new[] {skillId1, skillId3},
            Bonuses = new[] {_mapper.ToResponse(bonus)},
            Requirements = new[] {_mapper.ToResponse(requirement1), _mapper.ToResponse(requirement2)},
            Restrictions = new[] {_mapper.ToResponse(restriction)},
            Specialities = new[] {specialityResponse},
        }, o => o.WithStrictOrdering());
        actual.Data.ForOrigin.ToString(Newtonsoft.Json.Formatting.None).Should().Be("""{"all":{"diceEvLevelUp":4}}""");
    }

    [Test]
    public void ToResponse_JobWithoutFlags_ReturnsEmptyFlags()
    {
        var job = new JobEntity
        {
            Name = "some-name",
            Data = "{}",
            Flags = null,
            Skills = [],
            Bonuses = [],
            Requirements = [],
            Restrictions = [],
            Specialities = [],
        };

        var actual = _mapper.ToResponse(job);

        actual.Flags.Should().NotBeNull().And.BeEmpty();
    }

    [Test]
    public void ToResponse_JobBonus_MapsDescriptionAndFlags()
    {
        var bonus = new JobBonusEntity {Description = "some-description", Flags = """[{"type":"some-flag"}]"""};

        var actual = _mapper.ToResponse(bonus);

        actual.Should().BeEquivalentTo(new DescribedFlagResponse
        {
            Description = "some-description",
            Flags = [new FlagResponse {Type = "some-flag"}],
        });
    }

    [Test]
    public void ToResponse_JobRequirement_MapsStatAndMinMax()
    {
        var requirement = new JobRequirementEntity {StatName = "AD", MinValue = 8, MaxValue = 12};

        var actual = _mapper.ToResponse(requirement);

        actual.Should().BeEquivalentTo(new StatRequirementResponse
        {
            Stat = "AD",
            Min = 8,
            Max = 12,
        });
    }

    [Test]
    public void ToResponse_JobRestriction_MapsTextToDescriptionAndFlags()
    {
        var restriction = new JobRestrictionEntity {Text = "some-text", Flags = null};

        var actual = _mapper.ToResponse(restriction);

        actual.Should().BeEquivalentTo(new DescribedFlagResponse
        {
            Description = "some-text",
            Flags = [],
        });
    }
}
