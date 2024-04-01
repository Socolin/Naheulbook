using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Naheulbook.Core.Services;
using Naheulbook.Web.Responses;

namespace Naheulbook.Web.Controllers;

[Route("api/v2/stats")]
[ApiController]
public class StatsController(
    IMapper mapper,
    IStatService statService
) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<StatResponse>>> GetAllStatsAsync()
    {
        var stats = await statService.GetAllStatsAsync();
        return mapper.Map<List<StatResponse>>(stats);
    }
}