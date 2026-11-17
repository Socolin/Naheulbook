using Microsoft.AspNetCore.Mvc;
using Naheulbook.Core.Features.Effect;
using Naheulbook.Core.Features.Shared;
using Naheulbook.Requests.Requests;
using Naheulbook.Responses.Responses;
using Naheulbook.Web.Exceptions;

namespace Naheulbook.Web.Controllers;

[Route("api/v2/effectTypes")]
[ApiController]
public class EffectTypesController(IEffectService effectService, IEffectMapper effectMapper) : ControllerBase
{
    [HttpPost]
    public async Task<JsonResult> PostCreateTypeAsync(
        [FromServices] NaheulbookExecutionContext executionContext,
        CreateEffectTypeRequest request,
        CancellationToken cancellationToken
    )
    {
        try
        {
            var effectType = await effectService.CreateEffectTypeAsync(executionContext, request, cancellationToken);
            return new JsonResult(effectMapper.ToResponse(effectType))
            {
                StatusCode = 201,
            };
        }
        catch (ForbiddenAccessException ex)
        {
            throw new HttpErrorException(StatusCodes.Status403Forbidden, ex);
        }
    }
}