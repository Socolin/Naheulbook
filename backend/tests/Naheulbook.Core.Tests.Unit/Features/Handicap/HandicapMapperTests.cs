using AwesomeAssertions;
using Naheulbook.Core.Features.Handicap;
using Naheulbook.Data.EntityFrameworkCore.Entities;
using Naheulbook.Responses.Responses;
using Naheulbook.Shared.TransientModels;
using NUnit.Framework;

namespace Naheulbook.Core.Tests.Unit.Features.Handicap;

[TestFixture]
[TestOf(typeof(HandicapMapper))]
public class HandicapMapperTests
{
    private HandicapMapper _mapper;

    [SetUp]
    public void SetUp()
    {
        _mapper = new HandicapMapper();
    }

    [Test]
    public void ToResponse_Handicap_MapsAllFields()
    {
        var handicap = new HandicapEntity
        {
            Id = Guid.NewGuid(),
            DiceRoll = 42,
            Name = "some-name",
            Data = """{"condition": "some-condition", "effect": "some-effect"}""",
        };

        var actual = _mapper.ToResponse(handicap);

        actual.Should().BeEquivalentTo(new HandicapResponse
        {
            Id = handicap.Id,
            DiceRoll = 42,
            Name = "some-name",
            Data = new HandicapData
            {
                Condition = "some-condition",
                Effect = "some-effect",
            },
        });
    }
}
