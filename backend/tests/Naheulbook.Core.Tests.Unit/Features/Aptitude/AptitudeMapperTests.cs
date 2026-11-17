using AwesomeAssertions;
using Naheulbook.Core.Features.Aptitude;
using Naheulbook.Data.EntityFrameworkCore.Entities;
using Naheulbook.Responses.Responses;
using NUnit.Framework;

namespace Naheulbook.Core.Tests.Unit.Features.Aptitude;

[TestFixture]
[TestOf(typeof(AptitudeMapper))]
public class AptitudeMapperTests
{
    private AptitudeMapper _mapper;

    [SetUp]
    public void SetUp()
    {
        _mapper = new AptitudeMapper();
    }

    [Test]
    public void ToResponse_Aptitude_MapsAllFields()
    {
        var aptitude = CreateAptitude(Guid.NewGuid(), "some-name");

        var actual = _mapper.ToResponse(aptitude);

        actual.Should().BeEquivalentTo(new AptitudeResponse
        {
            Id = aptitude.Id,
            Name = "some-name",
            Roll = 4,
            Type = "some-type",
            Description = "some-description",
            Effect = "some-effect",
        });
    }

    [Test]
    public void ToSummaryResponse_AptitudeGroup_MapsIdAndName()
    {
        var aptitudeGroup = new AptitudeGroupEntity
        {
            Id = Guid.NewGuid(),
            Name = "some-group-name",
        };

        var actual = _mapper.ToSummaryResponse(aptitudeGroup);

        actual.Should().BeEquivalentTo(new SummaryAptitudeGroupResponse
        {
            Id = aptitudeGroup.Id,
            Name = "some-group-name",
        });
    }

    [Test]
    public void ToResponse_AptitudeGroup_MapsFieldsAndAptitudes()
    {
        var aptitude1 = CreateAptitude(Guid.NewGuid(), "some-name-1");
        var aptitude2 = CreateAptitude(Guid.NewGuid(), "some-name-2");
        var aptitudeGroup = new AptitudeGroupEntity
        {
            Id = Guid.NewGuid(),
            Name = "some-group-name",
            Aptitudes = [aptitude1, aptitude2],
        };

        var actual = _mapper.ToResponse(aptitudeGroup);

        actual.Should().BeEquivalentTo(new AptitudeGroupResponse
        {
            Id = aptitudeGroup.Id,
            Name = "some-group-name",
            Aptitudes = [_mapper.ToResponse(aptitude1), _mapper.ToResponse(aptitude2)],
        }, o => o.WithStrictOrdering());
    }

    [Test]
    public void ToResponse_CharacterAptitude_MapsAptitudeCountAndActive()
    {
        var aptitude = CreateAptitude(Guid.NewGuid(), "some-name");
        var characterAptitude = new CharacterAptitudeEntity
        {
            CharacterId = 8,
            AptitudeId = aptitude.Id,
            Aptitude = aptitude,
            Count = 3,
            Active = true,
        };

        var actual = _mapper.ToResponse(characterAptitude);

        actual.Should().BeEquivalentTo(new CharacterAptitudeResponse
        {
            Aptitude = _mapper.ToResponse(aptitude),
            Count = 3,
            Active = true,
        });
    }

    [Test]
    public void ToAddResponse_CharacterAptitude_MapsAptitudeCountAndActive()
    {
        var aptitude = CreateAptitude(Guid.NewGuid(), "some-name");
        var characterAptitude = new CharacterAptitudeEntity
        {
            CharacterId = 8,
            AptitudeId = aptitude.Id,
            Aptitude = aptitude,
            Count = 3,
            Active = true,
        };

        var actual = _mapper.ToAddResponse(characterAptitude);

        actual.Should().BeEquivalentTo(new CharacterAddAptitudeResponse
        {
            Aptitude = _mapper.ToResponse(aptitude),
            Count = 3,
            Active = true,
        });
    }

    private static AptitudeEntity CreateAptitude(Guid id, string name)
    {
        return new AptitudeEntity
        {
            Id = id,
            Name = name,
            Roll = 4,
            Type = "some-type",
            Description = "some-description",
            Effect = "some-effect",
        };
    }
}
