using AwesomeAssertions;
using Naheulbook.Core.Features.Calendar;
using Naheulbook.Data.EntityFrameworkCore.Entities;
using Naheulbook.Responses.Responses;
using NUnit.Framework;

namespace Naheulbook.Core.Tests.Unit.Features.Calendar;

[TestFixture]
[TestOf(typeof(CalendarMapper))]
public class CalendarMapperTests
{
    private CalendarMapper _mapper;

    [SetUp]
    public void SetUp()
    {
        _mapper = new CalendarMapper();
    }

    [Test]
    public void ToResponse_Calendar_MapsAllFields()
    {
        var calendar = new CalendarEntity
        {
            Id = 3,
            Name = "some-name",
            StartDay = 10,
            EndDay = 40,
            Note = "some-note",
        };

        var actual = _mapper.ToResponse(calendar);

        actual.Should().BeEquivalentTo(new CalendarResponse
        {
            Id = 3,
            Name = "some-name",
            StartDay = 10,
            EndDay = 40,
            Note = "some-note",
        });
    }
}
