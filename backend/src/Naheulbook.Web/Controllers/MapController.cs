using AutoMapper;
using BrunoZell.ModelBinding;
using Microsoft.AspNetCore.Mvc;
using Naheulbook.Core.Features.Map;
using Naheulbook.Core.Features.Shared;
using Naheulbook.Requests.Requests;
using Naheulbook.Web.Exceptions;
using Naheulbook.Web.Responses;

namespace Naheulbook.Web.Controllers;

[Route("/api/v2/maps")]
[ApiController]
public class MapController(
    IMapper mapper,
    IMapService mapService
) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<MapSummaryResponse>>> GetMapListAsync()
    {
        var maps = await mapService.GetMapsAsync();

        return mapper.Map<List<MapSummaryResponse>>(maps);
    }

    [HttpGet("{MapId:int:min(1)}")]
    public async Task<ActionResult<MapResponse>> GetMapAsync(
        [FromServices] OptionalNaheulbookExecutionContext executionContext,
        [FromRoute] int mapId
    )
    {
        try
        {
            var map = await mapService.GetMapAsync(mapId, executionContext.ExecutionExecutionContext?.UserId);

            return mapper.Map<MapResponse>(map);
        }
        catch (MapNotFoundException ex)
        {
            throw new HttpErrorException(StatusCodes.Status404NotFound, ex);
        }
    }

    [HttpPost]
    public async Task<ActionResult<MapResponse>> PostCreateMapAsync(
        [FromServices] NaheulbookExecutionContext executionContext,
        [ModelBinder(BinderType = typeof(JsonModelBinder), Name = "request")]
        CreateMapRequest request,
        [FromForm(Name = "image")] IFormFile image
    )
    {
        if (!image.ContentType.StartsWith("image/"))
            return BadRequest();

        var imageStream = image.OpenReadStream();
        var map = await mapService.CreateMapAsync(executionContext, request, imageStream);

        return mapper.Map<MapResponse>(map);
    }

    [HttpPut("{MapId:int:min(1)}")]
    public async Task<ActionResult<MapSummaryResponse>> PutUpdateMapAsync(
        [FromServices] NaheulbookExecutionContext executionContext,
        [FromRoute] int mapId,
        [FromBody] CreateMapRequest request
    )
    {
        var map = await mapService.UpdateMapAsync(executionContext, mapId, request);

        return mapper.Map<MapSummaryResponse>(map);
    }


    [HttpPost("{MapId:int:min(1)}/layers")]
    public async Task<ActionResult<MapLayerResponse>> PostCreateMapLayerAsync(
        [FromServices] NaheulbookExecutionContext executionContext,
        [FromRoute] int mapId,
        [FromBody] MapLayerRequest request
    )
    {
        var map = await mapService.CreateMapLayerAsync(executionContext, mapId, request);

        return mapper.Map<MapLayerResponse>(map);
    }
}