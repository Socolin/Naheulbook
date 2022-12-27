using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Naheulbook.Core.Services;
using Naheulbook.Web.Responses;

namespace Naheulbook.Web.Controllers;

[Route("api/v2/stats")]
[ApiController]
public class StatsController : ControllerBase
{
    private readonly IMapper _mapper;
    private readonly IStatService _statService;

    public StatsController(
        IMapper mapper,
        IStatService statService
    )
    {
        _mapper = mapper;
        _statService = statService;
    }

    [HttpGet]
    public async Task<ActionResult<List<StatResponse>>> GetAllStatsAsync()
    {
        var stats = await _statService.GetAllStatsAsync();
        return _mapper.Map<List<StatResponse>>(stats);
    }
}