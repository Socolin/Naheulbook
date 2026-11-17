using Microsoft.AspNetCore.Mvc;
using Naheulbook.Core.Features.Stat;
using Naheulbook.Responses.Responses;

namespace Naheulbook.Web.Controllers;

[Route("api/v2/stats")]
[ApiController]
public class StatsController(
    IStatMapper statMapper,
    IStatService statService
) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<StatResponse>>> GetAllStatsAsync()
    {
        var stats = await statService.GetAllStatsAsync();
        return stats.Select(statMapper.ToResponse).ToList();
    }
}