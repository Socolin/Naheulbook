using Microsoft.AspNetCore.Mvc;
using Naheulbook.Core.Features.Calendar;
using Naheulbook.Responses.Responses;

namespace Naheulbook.Web.Controllers;

[Route("api/v2/calendar")]
[ApiController]
public class CalendarController(
    ICalendarService calendarService,
    ICalendarMapper calendarMapper
)
{
    [HttpGet]
    public async Task<ActionResult<List<CalendarResponse>>> GetCalendarAsync(CancellationToken cancellationToken)
    {
        var calendar = await calendarService.GetCalendarAsync(cancellationToken);
        return calendar.Select(calendarMapper.ToResponse).ToList();
    }
}