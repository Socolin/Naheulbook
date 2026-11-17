using Microsoft.AspNetCore.Mvc;
using Naheulbook.Core.Features.Aptitude;
using Naheulbook.Responses.Responses;
using Naheulbook.Web.Exceptions;

namespace Naheulbook.Web.Controllers;

[Route("api/v2/aptitudeGroups")]
[ApiController]
public class AptitudeGroupController(
    IAptitudeGroupService aptitudeGroupService,
    IAptitudeMapper aptitudeMapper
) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<AptitudeGroupsResponse>> GetAptitudeGroupsAsync(
        CancellationToken cancellationToken = default
    )
    {
        var aptitudeGroups = await aptitudeGroupService.GetAptitudeGroupListAsync(cancellationToken);
        return new AptitudeGroupsResponse
        {
            AptitudeGroups = aptitudeGroups.Select(aptitudeMapper.ToSummaryResponse).ToList(),
        };
    }

    [HttpGet("{aptitudeGroupId:guid}")]
    public async Task<ActionResult<AptitudeGroupResponse>> GetAptitudeGroupsAsync(
        [FromRoute] Guid aptitudeGroupId,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            var aptitudeGroup = await aptitudeGroupService.GetAptitudeGroupAsync(aptitudeGroupId, cancellationToken);
            return aptitudeMapper.ToResponse(aptitudeGroup);
        }
        catch (AptitudeGroupNotFoundException ex)
        {
            throw new HttpErrorException(StatusCodes.Status404NotFound, ex);
        }
    }
}