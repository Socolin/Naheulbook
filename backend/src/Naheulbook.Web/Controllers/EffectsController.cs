using Microsoft.AspNetCore.Mvc;
using Naheulbook.Core.Features.Effect;
using Naheulbook.Core.Features.Shared;
using Naheulbook.Requests.Requests;
using Naheulbook.Responses.Responses;
using Naheulbook.Web.Exceptions;

namespace Naheulbook.Web.Controllers;

[ApiController]
[Route("api/v2/effects")]
public class EffectsController(IEffectService effectService, IEffectMapper effectMapper) : ControllerBase
{
    [HttpPut("{effectId:int:min(1)}")]
    public async Task<ActionResult<EffectResponse>> PutEditEffectAsync(
        [FromServices] NaheulbookExecutionContext executionContext,
        [FromRoute] int effectId,
        EditEffectRequest request
    )
    {
        try
        {
            var effect = await effectService.EditEffectAsync(executionContext, effectId, request);
            var effectResponse = effectMapper.ToResponse(effect);
            return effectResponse;
        }
        catch (ForbiddenAccessException ex)
        {
            throw new HttpErrorException(StatusCodes.Status403Forbidden, ex);
        }
        catch (EffectNotFoundException ex)
        {
            throw new HttpErrorException(StatusCodes.Status404NotFound, ex);
        }
    }

    [HttpGet("{effectId:int:min(1)}")]
    public async Task<ActionResult<EffectResponse>> GetEffectAsync(
        [FromRoute] int effectId
    )
    {
        try
        {
            var effect = await effectService.GetEffectAsync(effectId);
            var effectResponse = effectMapper.ToResponse(effect);
            return effectResponse;
        }
        catch (EffectNotFoundException ex)
        {
            throw new HttpErrorException(StatusCodes.Status404NotFound, ex);
        }
    }

    [HttpGet("search")]
    public async Task<ActionResult<List<EffectResponse>>> GetSearchEffectAsync(
        [FromQuery] string filter
    )
    {
        var effects = await effectService.SearchEffectsAsync(filter);
        return effects.Select(effectMapper.ToResponse).ToList();
    }
}