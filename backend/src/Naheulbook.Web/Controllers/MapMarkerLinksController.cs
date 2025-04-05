using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Naheulbook.Core.Features.Map;
using Naheulbook.Core.Features.Shared;
using Naheulbook.Requests.Requests;
using Naheulbook.Web.Exceptions;
using Naheulbook.Web.Responses;

namespace Naheulbook.Web.Controllers;

[Route("/api/v2/mapMarkerLinks")]
[ApiController]
public class MapMarkerLinksController(IMapService mapService, IMapper mapper) : ControllerBase
{
    [HttpPut("{MapMarkerLinkId}")]
    public async Task<ActionResult<MapMarkerLinkResponse>> PutEditMapMarkerLinkAsync(
        [FromServices] NaheulbookExecutionContext executionContext,
        [FromRoute] int mapMarkerLinkId,
        MapMarkerLinkRequest request
    )
    {
        try
        {
            var mapMarkerLink = await mapService.EditMapMarkerLinkAsync(executionContext, mapMarkerLinkId, request);
            return mapper.Map<MapMarkerLinkResponse>(mapMarkerLink);
        }
        catch (MapMarkerNotFoundException ex)
        {
            throw new HttpErrorException(StatusCodes.Status404NotFound, ex);
        }
    }

    [HttpDelete("{MapMarkerLinkId}")]
    public async Task<IActionResult> DeleteMarkerLinkAsync(
        [FromServices] NaheulbookExecutionContext executionContext,
        [FromRoute] int mapMarkerLinkId
    )
    {
        try
        {
            await mapService.DeleteMapMarkerLinkAsync(executionContext, mapMarkerLinkId);
        }
        catch (MapMarkerNotFoundException ex)
        {
            throw new HttpErrorException(StatusCodes.Status404NotFound, ex);
        }

        return NoContent();
    }
}