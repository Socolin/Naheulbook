using Naheulbook.Data.EntityFrameworkCore.Entities;
using Naheulbook.Responses.Responses;

namespace Naheulbook.Core.Features.Calendar;

public interface ICalendarMapper
{
    CalendarResponse ToResponse(CalendarEntity calendar);
}

public class CalendarMapper : ICalendarMapper
{
    public CalendarResponse ToResponse(CalendarEntity calendar)
    {
        return new CalendarResponse
        {
            Id = calendar.Id,
            Name = calendar.Name,
            StartDay = calendar.StartDay,
            EndDay = calendar.EndDay,
            Note = calendar.Note,
        };
    }
}
