using Microsoft.AspNetCore.Mvc;
using Naheulbook.Core.Features.Map;
using Naheulbook.Core.Features.Shared;
using Naheulbook.Requests.Requests;
using Naheulbook.Responses.Responses;
using Naheulbook.Web.Exceptions;

namespace Naheulbook.Web.Controllers;

[Route("/api/v2/mapMarkers")]
[ApiController]
public class MapMarkerController(IMapService mapService, IMapMapper mapMapper) : ControllerBase
{
    [HttpDelete("{MapMarkerId}")]
    public async Task<IActionResult> DeleteMarkerAsync(
        [FromServices] NaheulbookExecutionContext naheulbookExecutionContext,
        [FromRoute] int mapMarkerId,
        CancellationToken cancellationToken
    )
    {
        try
        {
            await mapService.DeleteMapMarkerAsync(naheulbookExecutionContext, mapMarkerId, cancellationToken);
        }
        catch (MapMarkerNotFoundException ex)
        {
            throw new HttpErrorException(StatusCodes.Status404NotFound, ex);
        }

        return NoContent();
    }

    [HttpPut("{MapMarkerId}")]
    public async Task<ActionResult<MapMarkerResponse>> PutMarkerAsync(
        [FromServices] NaheulbookExecutionContext naheulbookExecutionContext,
        [FromRoute] int mapMarkerId,
        [FromBody] MapMarkerRequest request,
        CancellationToken cancellationToken
    )
    {
        try
        {
            var marker = await mapService.EditMapMarkerAsync(naheulbookExecutionContext, mapMarkerId, request, cancellationToken);
            return mapMapper.ToResponse(marker);
        }
        catch (MapMarkerNotFoundException ex)
        {
            throw new HttpErrorException(StatusCodes.Status404NotFound, ex);
        }
    }

    [HttpPost("{MapMarkerId}/links")]
    public async Task<ActionResult<MapMarkerLinkResponse>> CreateMapMarkerLinkAsync(
        [FromServices] NaheulbookExecutionContext naheulbookExecutionContext,
        [FromRoute] int mapMarkerId,
        [FromBody] MapMarkerLinkRequest request,
        CancellationToken cancellationToken
    )
    {
        try
        {
            var marker = await mapService.CreateMapMarkerLinkAsync(naheulbookExecutionContext, mapMarkerId, request, cancellationToken);
            return mapMapper.ToResponse(marker);
        }
        catch (MapMarkerNotFoundException ex)
        {
            throw new HttpErrorException(StatusCodes.Status404NotFound, ex);
        }
    }
}