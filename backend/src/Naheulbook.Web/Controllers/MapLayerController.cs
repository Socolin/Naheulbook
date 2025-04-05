using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Naheulbook.Core.Features.Map;
using Naheulbook.Core.Features.Shared;
using Naheulbook.Requests.Requests;
using Naheulbook.Web.Responses;

namespace Naheulbook.Web.Controllers;

[Route("/api/v2/mapLayers")]
[ApiController]
public class MapLayerController(
    IMapper mapper,
    IMapService mapService
) : ControllerBase
{
    [HttpPost("{MapLayerId:int:min(1)}/markers")]
    public async Task<ActionResult<MapMarkerResponse>> PostCreateMapMarkerAsync(
        [FromServices] NaheulbookExecutionContext executionContext,
        [FromRoute] int mapLayerId,
        [FromBody] MapMarkerRequest request
    )
    {
        var map = await mapService.CreateMapMarkerAsync(executionContext, mapLayerId, request);

        return mapper.Map<MapMarkerResponse>(map);
    }

    [HttpPut("{MapLayerId:int:min(1)}")]
    public async Task<ActionResult<MapLayerResponse>> PutEditMapMarkerAsync(
        [FromServices] NaheulbookExecutionContext executionContext,
        [FromRoute] int mapLayerId,
        [FromBody] MapLayerRequest request
    )
    {
        var map = await mapService.EditMapLayerAsync(executionContext, mapLayerId, request);

        return mapper.Map<MapLayerResponse>(map);
    }

    [HttpDelete("{MapLayerId:int:min(1)}")]
    public async Task<IActionResult> DeleteMapLayerAsync(
        [FromServices] NaheulbookExecutionContext executionContext,
        [FromRoute] int mapLayerId
    )
    {
        await mapService.DeleteMapLayerAsync(executionContext, mapLayerId);

        return NoContent();
    }
}