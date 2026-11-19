using AwesomeAssertions;
using Naheulbook.Core.Features.Talent;
using Naheulbook.Data.EntityFrameworkCore.Entities;
using Naheulbook.Responses.Responses;
using Naheulbook.Shared.TransientModels;
using NUnit.Framework;

namespace Naheulbook.Core.Tests.Unit.Features.Talent;

[TestFixture]
[TestOf(typeof(TalentMapper))]
public class TalentMapperTests
{
    private static readonly Guid PrerequisiteTalentId = Guid.Parse("0b6f5c2e-4d1a-4e8b-9c3f-7a2d1e5b8c90");
    private TalentMapper _mapper;

    [SetUp]
    public void SetUp()
    {
        _mapper = new TalentMapper();
    }

    [Test]
    public void ToResponse_Talent_MapsAllFields()
    {
        var talent = CreateTalent("some-name", 1);

        var actual = _mapper.ToResponse(talent);

        actual.Should().BeEquivalentTo(new TalentResponse
        {
            Id = talent.Id,
            Name = "some-name",
            Data = new TalentData
            {
                Restriction = "some-restriction",
                Effect = "some-effect",
                Notes = "some-notes",
                RequiredLevel = 10,
                PrerequisiteTalentId = PrerequisiteTalentId,
            },
        });
    }

    [Test]
    public void ToResponse_TalentGroup_MapsFieldsAndTalentsOrderedByDisplayOrder()
    {
        var talent2 = CreateTalent("some-name-2", 2);
        var talent1 = CreateTalent("some-name-1", 1);
        var talentGroup = new TalentGroupEntity
        {
            Id = Guid.NewGuid(),
            Name = "some-group-name",
            Description = "some-group-description",
            Talents = [talent2, talent1],
        };

        var actual = _mapper.ToResponse(talentGroup);

        actual.Id.Should().Be(talentGroup.Id);
        actual.Name.Should().Be("some-group-name");
        actual.Description.Should().Be("some-group-description");
        actual.Talents.Select(x => x.Name).Should().Equal("some-name-1", "some-name-2");
    }

    private static TalentEntity CreateTalent(string name, int displayOrder)
    {
        return new TalentEntity
        {
            Id = Guid.NewGuid(),
            DisplayOrder = displayOrder,
            Name = name,
            Data = """
                   {
                     "restriction": "some-restriction",
                     "effect": "some-effect",
                     "notes": "some-notes",
                     "requiredLevel": 10,
                     "prerequisiteTalentId": "0b6f5c2e-4d1a-4e8b-9c3f-7a2d1e5b8c90"
                   }
                   """,
        };
    }
}
