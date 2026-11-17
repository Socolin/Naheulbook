using AwesomeAssertions;
using Naheulbook.Core.Features.Stat;
using Naheulbook.Data.EntityFrameworkCore.Entities;
using Naheulbook.Responses.Responses;
using NUnit.Framework;

namespace Naheulbook.Core.Tests.Unit.Features.Stat;

[TestFixture]
[TestOf(typeof(StatMapper))]
public class StatMapperTests
{
    private StatMapper _mapper;

    [SetUp]
    public void SetUp()
    {
        _mapper = new StatMapper();
    }

    [Test]
    public void ToResponse_Stat_MapsAllFields()
    {
        var stat = new StatEntity
        {
            Name = "FO",
            DisplayName = "some-display-name",
            Description = "some-description",
            Bonus = "some-bonus",
            Penalty = "some-penalty",
        };

        var actual = _mapper.ToResponse(stat);

        actual.Should().BeEquivalentTo(new StatResponse
        {
            Name = "FO",
            DisplayName = "some-display-name",
            Description = "some-description",
        });
    }
}
