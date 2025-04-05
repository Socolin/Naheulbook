using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Naheulbook.Core.Features.Calendar;
using Naheulbook.Web.Responses;

namespace Naheulbook.Web.Controllers;

[Route("api/v2/calendar")]
[ApiController]
public class CalendarController(
    ICalendarService calendarService,
    IMapper mapper
)
{
    [HttpGet]
    public async Task<ActionResult<List<CalendarResponse>>> GetCalendarAsync()
    {
        var calendar = await calendarService.GetCalendarAsync();
        return mapper.Map<List<CalendarResponse>>(calendar);
    }
}