using AwesomeAssertions;
using Naheulbook.Core.Features.Job;
using Naheulbook.Data.EntityFrameworkCore.Entities;
using Naheulbook.Responses.Responses;
using NUnit.Framework;

namespace Naheulbook.Core.Tests.Unit.Features.Job;

[TestFixture]
[TestOf(typeof(SpecialityMapper))]
public class SpecialityMapperTests
{
    private SpecialityMapper _mapper;

    [SetUp]
    public void SetUp()
    {
        _mapper = new SpecialityMapper();
    }

    [Test]
    public void ToResponse_Speciality_MapsFieldsAndOrdersModifiersAndSpecialsById()
    {
        var modifier1 = new SpecialityModifierEntity {Id = 1, Stat = "AD", Value = 1};
        var modifier2 = new SpecialityModifierEntity {Id = 2, Stat = "FO", Value = 2};
        var special1 = new SpecialitySpecialEntity {Id = 1, Description = "some-description-1"};
        var special2 = new SpecialitySpecialEntity {Id = 2, Description = "some-description-2"};
        var speciality = new SpecialityEntity
        {
            Id = Guid.NewGuid(),
            Name = "some-name",
            Description = "some-description",
            Flags = """[{"type":"some-flag"}]""",
            Modifiers = [modifier2, modifier1],
            Specials = [special2, special1],
        };

        var actual = _mapper.ToResponse(speciality);

        actual.Should().BeEquivalentTo(new SpecialityResponse
        {
            Id = speciality.Id,
            Name = "some-name",
            Description = "some-description",
            Flags = [new FlagResponse {Type = "some-flag"}],
            Modifiers = [_mapper.ToResponse(modifier1), _mapper.ToResponse(modifier2)],
            Specials = [_mapper.ToResponse(special1), _mapper.ToResponse(special2)],
        }, o => o.WithStrictOrdering());
    }

    [Test]
    public void ToResponse_SpecialityWithoutFlags_ReturnsEmptyFlags()
    {
        var speciality = new SpecialityEntity
        {
            Name = "some-name",
            Description = "some-description",
            Flags = null,
            Modifiers = [],
            Specials = [],
        };

        var actual = _mapper.ToResponse(speciality);

        actual.Flags.Should().NotBeNull().And.BeEmpty();
    }

    [Test]
    public void ToResponse_SpecialityModifier_MapsStatAndUseAddType()
    {
        var modifier = new SpecialityModifierEntity {Id = 4, Stat = "INT", Value = 3};

        var actual = _mapper.ToResponse(modifier);

        actual.Should().BeEquivalentTo(new StatModifierResponse
        {
            Stat = "INT",
            Type = "ADD",
            Value = 3,
            Special = null,
        });
    }

    [Test]
    public void ToResponse_SpecialitySpecial_MapsAllFields()
    {
        var special = new SpecialitySpecialEntity
        {
            Id = 4,
            IsBonus = true,
            Description = "some-description",
            Flags = """[{"type":"some-flag"}]""",
        };

        var actual = _mapper.ToResponse(special);

        actual.Should().BeEquivalentTo(new SpecialitySpecialResponse
        {
            Id = 4,
            IsBonus = true,
            Description = "some-description",
            Flags = [new FlagResponse {Type = "some-flag"}],
        });
    }
}
