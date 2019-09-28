using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Naheulbook.Core.Services;
using Naheulbook.Web.Responses;

namespace Naheulbook.Web.Controllers
{
    [Route("api/v2/calendar")]
    [ApiController]
    public class CalendarController
    {
        private readonly ICalendarService _calendarService;
        private readonly IMapper _mapper;

        public CalendarController(
            ICalendarService calendarService,
            IMapper mapper
        )
        {
            _calendarService = calendarService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<CalendarResponse>>> GetCalendarAsync()
        {
            var calendar = await _calendarService.GetCalendarAsync();
            return _mapper.Map<List<CalendarResponse>>(calendar);
        }
    }
}