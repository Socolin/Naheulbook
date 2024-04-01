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

[Route("api/v2/effectTypes")]
[ApiController]
public class EffectTypesController(IEffectService effectService, IMapper mapper) : ControllerBase
{
    [HttpPost]
    public async Task<JsonResult> PostCreateTypeAsync(
        [FromServices] NaheulbookExecutionContext executionContext,
        CreateEffectTypeRequest request
    )
    {
        try
        {
            var effectType = await effectService.CreateEffectTypeAsync(executionContext, request);
            return new JsonResult(mapper.Map<EffectTypeResponse>(effectType))
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