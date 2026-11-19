using Microsoft.AspNetCore.Mvc;
using Naheulbook.Core.Features.Mastery;
using Naheulbook.Responses.Responses;

namespace Naheulbook.Web.Controllers;

[Route("api/v2/masteryGroups")]
[ApiController]
public class MasteryGroupController(
    IMasteryService masteryService,
    IMasteryMapper masteryMapper
) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<MasteryGroupsResponse>> GetMasteryGroupsAsync(
        CancellationToken cancellationToken
    )
    {
        var masteryGroups = await masteryService.GetMasteryGroupsAsync(cancellationToken);
        return new MasteryGroupsResponse
        {
            MasteryGroups = masteryGroups.Select(masteryMapper.ToResponse).ToList(),
        };
    }
}
