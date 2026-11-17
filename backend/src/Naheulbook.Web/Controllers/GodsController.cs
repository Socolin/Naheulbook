using Microsoft.AspNetCore.Mvc;
using Naheulbook.Core.Features.God;
using Naheulbook.Responses.Responses;

namespace Naheulbook.Web.Controllers;

[Route("api/v2/gods")]
[ApiController]
public class GodsController(
    IGodMapper godMapper,
    IGodService godService
) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<GodResponse>>> GetAllGodsAsync()
    {
        var gods = await godService.GetAllGodsAsync();
        return gods.Select(godMapper.ToResponse).ToList();
    }
}