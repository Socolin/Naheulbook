using AwesomeAssertions;
using Naheulbook.Core.Features.Skill;
using Naheulbook.Data.EntityFrameworkCore.Entities;
using Naheulbook.Responses.Responses;
using NUnit.Framework;

namespace Naheulbook.Core.Tests.Unit.Features.Skill;

[TestFixture]
[TestOf(typeof(SkillMapper))]
public class SkillMapperTests
{
    private SkillMapper _mapper;

    [SetUp]
    public void SetUp()
    {
        _mapper = new SkillMapper();
    }

    [Test]
    public void ToResponse_Skill_MapsAllFields()
    {
        var skillEffect = new SkillEffectEntity {StatName = "AD", Value = 2};
        var skill = new SkillEntity
        {
            Id = Guid.NewGuid(),
            Name = "some-name",
            Description = "some-description",
            PlayerDescription = "some-player-description",
            Require = "some-require",
            Resist = "some-resist",
            Using = "some-using",
            Roleplay = "some-roleplay",
            Stat = "AD,FO",
            Test = 4,
            Flags = """[{"type":"some-flag"}]""",
            SkillEffects = [skillEffect],
        };

        var actual = _mapper.ToResponse(skill);

        actual.Should().BeEquivalentTo(new SkillResponse
        {
            Id = skill.Id,
            Name = "some-name",
            Description = "some-description",
            PlayerDescription = "some-player-description",
            Require = "some-require",
            Resist = "some-resist",
            Using = "some-using",
            Roleplay = "some-roleplay",
            Stat = ["AD", "FO"],
            Test = 4,
            Flags = [new FlagResponse {Type = "some-flag"}],
            Effects = [_mapper.ToResponse(skillEffect)],
        });
    }

    [Test]
    public void ToResponse_SkillWithoutStatAndFlags_ReturnsEmptyLists()
    {
        var skill = new SkillEntity
        {
            Name = "some-name",
            Stat = null,
            Flags = null,
            SkillEffects = [],
        };

        var actual = _mapper.ToResponse(skill);

        actual.Stat.Should().BeEmpty();
        actual.Flags.Should().NotBeNull().And.BeEmpty();
    }

    [Test]
    public void ToResponse_SkillEffect_MapsStatNameAndUseAddType()
    {
        var skillEffect = new SkillEffectEntity {StatName = "CHA", Value = -1};

        var actual = _mapper.ToResponse(skillEffect);

        actual.Should().BeEquivalentTo(new SkillEffectResponse
        {
            Stat = "CHA",
            Value = -1,
            Type = "ADD",
        });
    }
}
