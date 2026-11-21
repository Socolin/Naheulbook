using AwesomeAssertions;
using Naheulbook.Core.Features.Entropy;
using Naheulbook.Data.EntityFrameworkCore.Entities;
using Naheulbook.Responses.Responses;
using NUnit.Framework;

namespace Naheulbook.Core.Tests.Unit.Features.Entropy;

[TestFixture]
[TestOf(typeof(EntropyMapper))]
public class EntropyMapperTests
{
    private EntropyMapper _mapper;

    [SetUp]
    public void SetUp()
    {
        _mapper = new EntropyMapper();
    }

    [Test]
    public void ToResponse_MapsAllFields()
    {
        var blessing = new DebilibeukBlessingEntity
        {
            Id = Guid.NewGuid(),
            DiceRoll = 42,
            Name = "some-name",
            Effect = "some-effect",
        };

        var actual = _mapper.ToResponse(blessing);

        actual.Should().BeEquivalentTo(new EntropyEventResponse
        {
            Id = blessing.Id,
            DiceRoll = 42,
            Name = "some-name",
            Effect = "some-effect",
        });
    }
}
