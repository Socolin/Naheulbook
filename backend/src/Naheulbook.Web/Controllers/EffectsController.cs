using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Naheulbook.Core.Exceptions;
using Naheulbook.Core.Models;
using Naheulbook.Core.Services;
using Naheulbook.Requests.Requests;
using Naheulbook.Web.Exceptions;
using Naheulbook.Web.Responses;

namespace Naheulbook.Web.Controllers;

[ApiController]
[Route("api/v2/effects")]
public class EffectsController(IEffectService effectService, IMapper mapper) : ControllerBase
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
            var effectResponse = mapper.Map<EffectResponse>(effect);
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
            var effectResponse = mapper.Map<EffectResponse>(effect);
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
        return mapper.Map<List<EffectResponse>>(effects);
    }
}