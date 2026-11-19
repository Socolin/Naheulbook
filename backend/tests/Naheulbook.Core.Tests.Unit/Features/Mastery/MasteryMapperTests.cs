using AwesomeAssertions;
using Naheulbook.Core.Features.Mastery;
using Naheulbook.Data.EntityFrameworkCore.Entities;
using Naheulbook.Responses.Responses;
using Naheulbook.Shared.TransientModels;
using NUnit.Framework;

namespace Naheulbook.Core.Tests.Unit.Features.Mastery;

[TestFixture]
[TestOf(typeof(MasteryMapper))]
public class MasteryMapperTests
{
    private static readonly Guid PrerequisiteMasteryId = Guid.Parse("0b6f5c2e-4d1a-4e8b-9c3f-7a2d1e5b8c90");
    private MasteryMapper _mapper;

    [SetUp]
    public void SetUp()
    {
        _mapper = new MasteryMapper();
    }

    [Test]
    public void ToResponse_Mastery_MapsAllFields()
    {
        var mastery = CreateMastery("some-name", 1);

        var actual = _mapper.ToResponse(mastery);

        actual.Should().BeEquivalentTo(new MasteryResponse
        {
            Id = mastery.Id,
            Name = "some-name",
            Data = new MasteryData
            {
                Restriction = "some-restriction",
                Effect = "some-effect",
                Notes = "some-notes",
                RequiredLevel = 10,
                PrerequisiteMasteryId = PrerequisiteMasteryId,
            },
        });
    }

    [Test]
    public void ToResponse_MasteryGroup_MapsFieldsAndMasteriesOrderedByDisplayOrder()
    {
        var mastery2 = CreateMastery("some-name-2", 2);
        var mastery1 = CreateMastery("some-name-1", 1);
        var masteryGroup = new MasteryGroupEntity
        {
            Id = Guid.NewGuid(),
            Name = "some-group-name",
            Description = "some-group-description",
            Masteries = [mastery2, mastery1],
        };

        var actual = _mapper.ToResponse(masteryGroup);

        actual.Id.Should().Be(masteryGroup.Id);
        actual.Name.Should().Be("some-group-name");
        actual.Description.Should().Be("some-group-description");
        actual.Masteries.Select(x => x.Name).Should().Equal("some-name-1", "some-name-2");
    }

    private static MasteryEntity CreateMastery(string name, int displayOrder)
    {
        return new MasteryEntity
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
                     "prerequisiteMasteryId": "0b6f5c2e-4d1a-4e8b-9c3f-7a2d1e5b8c90"
                   }
                   """,
        };
    }
}
