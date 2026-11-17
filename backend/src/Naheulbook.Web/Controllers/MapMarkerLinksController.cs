using Microsoft.AspNetCore.Mvc;
using Naheulbook.Core.Features.Map;
using Naheulbook.Core.Features.Shared;
using Naheulbook.Requests.Requests;
using Naheulbook.Responses.Responses;
using Naheulbook.Web.Exceptions;

namespace Naheulbook.Web.Controllers;

[Route("/api/v2/mapMarkerLinks")]
[ApiController]
public class MapMarkerLinksController(IMapService mapService, IMapMapper mapMapper) : ControllerBase
{
    [HttpPut("{MapMarkerLinkId}")]
    public async Task<ActionResult<MapMarkerLinkResponse>> PutEditMapMarkerLinkAsync(
        [FromServices] NaheulbookExecutionContext executionContext,
        [FromRoute] int mapMarkerLinkId,
        MapMarkerLinkRequest request,
        CancellationToken cancellationToken
    )
    {
        try
        {
            var mapMarkerLink = await mapService.EditMapMarkerLinkAsync(executionContext, mapMarkerLinkId, request, cancellationToken);
            return mapMapper.ToResponse(mapMarkerLink);
        }
        catch (MapMarkerNotFoundException ex)
        {
            throw new HttpErrorException(StatusCodes.Status404NotFound, ex);
        }
    }

    [HttpDelete("{MapMarkerLinkId}")]
    public async Task<IActionResult> DeleteMarkerLinkAsync(
        [FromServices] NaheulbookExecutionContext executionContext,
        [FromRoute] int mapMarkerLinkId,
        CancellationToken cancellationToken
    )
    {
        try
        {
            await mapService.DeleteMapMarkerLinkAsync(executionContext, mapMarkerLinkId, cancellationToken);
        }
        catch (MapMarkerNotFoundException ex)
        {
            throw new HttpErrorException(StatusCodes.Status404NotFound, ex);
        }

        return NoContent();
    }
}