using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Naheulbook.Core.Features.Aptitude;
using Naheulbook.Web.Exceptions;
using Naheulbook.Web.Responses;

namespace Naheulbook.Web.Controllers;

[Route("api/v2/aptitudeGroups")]
[ApiController]
public class AptitudeGroupController(
    IAptitudeGroupService aptitudeGroupService,
    IMapper mapper
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
            AptitudeGroups = mapper.Map<List<SummaryAptitudeGroupResponse>>(aptitudeGroups),
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
            return mapper.Map<AptitudeGroupResponse>(aptitudeGroup);
        }
        catch (AptitudeGroupNotFoundException ex)
        {
            throw new HttpErrorException(StatusCodes.Status404NotFound, ex);
        }
    }
}