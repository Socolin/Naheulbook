using Microsoft.AspNetCore.Mvc;
using Naheulbook.Core.Features.Handicap;
using Naheulbook.Responses.Responses;

namespace Naheulbook.Web.Controllers;

[Route("api/v2/handicaps")]
[ApiController]
public class HandicapController(
    IHandicapService handicapService,
    IHandicapMapper handicapMapper
) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<HandicapsResponse>> GetHandicapsAsync(
        CancellationToken cancellationToken
    )
    {
        var handicaps = await handicapService.GetHandicapsAsync(cancellationToken);
        return new HandicapsResponse
        {
            Handicaps = handicaps.Select(handicapMapper.ToResponse).ToList(),
        };
    }
}
