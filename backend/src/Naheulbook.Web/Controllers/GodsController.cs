using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Naheulbook.Core.Features.God;
using Naheulbook.Web.Responses;

namespace Naheulbook.Web.Controllers;

[Route("api/v2/gods")]
[ApiController]
public class GodsController(
    IMapper mapper,
    IGodService godService
) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<GodResponse>>> GetAllGodsAsync()
    {
        var gods = await godService.GetAllGodsAsync();
        return mapper.Map<List<GodResponse>>(gods);
    }
}