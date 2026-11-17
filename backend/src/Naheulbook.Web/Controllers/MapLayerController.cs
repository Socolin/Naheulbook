using Microsoft.AspNetCore.Mvc;
using Naheulbook.Core.Features.Map;
using Naheulbook.Core.Features.Shared;
using Naheulbook.Requests.Requests;
using Naheulbook.Responses.Responses;

namespace Naheulbook.Web.Controllers;

[Route("/api/v2/mapLayers")]
[ApiController]
public class MapLayerController(
    IMapMapper mapMapper,
    IMapService mapService
) : ControllerBase
{
    [HttpPost("{MapLayerId:int:min(1)}/markers")]
    public async Task<ActionResult<MapMarkerResponse>> PostCreateMapMarkerAsync(
        [FromServices] NaheulbookExecutionContext executionContext,
        [FromRoute] int mapLayerId,
        [FromBody] MapMarkerRequest request,
        CancellationToken cancellationToken
    )
    {
        var map = await mapService.CreateMapMarkerAsync(executionContext, mapLayerId, request, cancellationToken);

        return mapMapper.ToResponse(map);
    }

    [HttpPut("{MapLayerId:int:min(1)}")]
    public async Task<ActionResult<MapLayerResponse>> PutEditMapMarkerAsync(
        [FromServices] NaheulbookExecutionContext executionContext,
        [FromRoute] int mapLayerId,
        [FromBody] MapLayerRequest request,
        CancellationToken cancellationToken
    )
    {
        var map = await mapService.EditMapLayerAsync(executionContext, mapLayerId, request, cancellationToken);

        return mapMapper.ToResponse(map);
    }

    [HttpDelete("{MapLayerId:int:min(1)}")]
    public async Task<IActionResult> DeleteMapLayerAsync(
        [FromServices] NaheulbookExecutionContext executionContext,
        [FromRoute] int mapLayerId,
        CancellationToken cancellationToken
    )
    {
        await mapService.DeleteMapLayerAsync(executionContext, mapLayerId, cancellationToken);

        return NoContent();
    }
}