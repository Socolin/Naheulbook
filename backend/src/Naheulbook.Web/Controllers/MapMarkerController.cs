using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Naheulbook.Core.Features.Map;
using Naheulbook.Core.Features.Shared;
using Naheulbook.Requests.Requests;
using Naheulbook.Web.Exceptions;
using Naheulbook.Web.Responses;

namespace Naheulbook.Web.Controllers;

[Route("/api/v2/mapMarkers")]
[ApiController]
public class MapMarkerController(IMapService mapService, IMapper mapper) : ControllerBase
{
    [HttpDelete("{MapMarkerId}")]
    public async Task<IActionResult> DeleteMarkerAsync(
        [FromServices] NaheulbookExecutionContext naheulbookExecutionContext,
        [FromRoute] int mapMarkerId
    )
    {
        try
        {
            await mapService.DeleteMapMarkerAsync(naheulbookExecutionContext, mapMarkerId);
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
        [FromBody] MapMarkerRequest request
    )
    {
        try
        {
            var marker = await mapService.EditMapMarkerAsync(naheulbookExecutionContext, mapMarkerId, request);
            return mapper.Map<MapMarkerResponse>(marker);
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
        [FromBody] MapMarkerLinkRequest request
    )
    {
        try
        {
            var marker = await mapService.CreateMapMarkerLinkAsync(naheulbookExecutionContext, mapMarkerId, request);
            return mapper.Map<MapMarkerLinkResponse>(marker);
        }
        catch (MapMarkerNotFoundException ex)
        {
            throw new HttpErrorException(StatusCodes.Status404NotFound, ex);
        }
    }
}