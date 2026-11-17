using AwesomeAssertions;
using Naheulbook.Core.Features.Event;
using Naheulbook.Data.EntityFrameworkCore.Entities;
using Naheulbook.Responses.Responses;
using NUnit.Framework;

namespace Naheulbook.Core.Tests.Unit.Features.Event;

[TestFixture]
[TestOf(typeof(EventMapper))]
public class EventMapperTests
{
    private EventMapper _mapper;

    [SetUp]
    public void SetUp()
    {
        _mapper = new EventMapper();
    }

    [Test]
    public void ToResponse_Event_MapsAllFields()
    {
        var groupEvent = new EventEntity
        {
            Id = 3,
            Name = "some-name",
            Description = "some-description",
            Timestamp = 123456,
            GroupId = 8,
        };

        var actual = _mapper.ToResponse(groupEvent);

        actual.Should().BeEquivalentTo(new EventResponse
        {
            Id = 3,
            Name = "some-name",
            Description = "some-description",
            Timestamp = 123456,
        });
    }
}
