using AwesomeAssertions;
using Naheulbook.Core.Features.God;
using Naheulbook.Data.EntityFrameworkCore.Entities;
using Naheulbook.Responses.Responses;
using NUnit.Framework;

namespace Naheulbook.Core.Tests.Unit.Features.God;

[TestFixture]
[TestOf(typeof(GodMapper))]
public class GodMapperTests
{
    private GodMapper _mapper;

    [SetUp]
    public void SetUp()
    {
        _mapper = new GodMapper();
    }

    [Test]
    public void ToResponse_God_MapsAllFields()
    {
        var god = new GodEntity
        {
            Id = 3,
            DisplayName = "some-display-name",
            Description = "some-description",
            TechName = "some-tech-name",
        };

        var actual = _mapper.ToResponse(god);

        actual.Should().BeEquivalentTo(new GodResponse
        {
            Id = 3,
            DisplayName = "some-display-name",
            Description = "some-description",
            TechName = "some-tech-name",
        });
    }
}
