using Microsoft.AspNetCore.Mvc;
using Naheulbook.Core.Features.Effect;
using Naheulbook.Core.Features.Shared;
using Naheulbook.Requests.Requests;
using Naheulbook.Responses.Responses;
using Naheulbook.Web.ActionResults;
using Naheulbook.Web.Exceptions;

namespace Naheulbook.Web.Controllers;

[Route("api/v2/effectSubCategories")]
[ApiController]
public class EffectSubCategoriesController(IEffectService effectService, IEffectMapper effectMapper) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<EffectTypeResponse>>> GetEffectSubCategoriesAsync()
    {
        var effectSubCategories = await effectService.GetEffectSubCategoriesAsync();
        return effectSubCategories.Select(effectMapper.ToResponse).ToList();
    }

    [HttpGet("{subCategoryId:int:min(1)}/effects")]
    public async Task<ActionResult<List<EffectResponse>>> GetEffectsAsync(long subCategoryId)
    {
        var effects = await effectService.GetEffectsBySubCategoryAsync(subCategoryId);
        return effects.Select(effectMapper.ToResponse).ToList();
    }

    [HttpPost("{subCategoryId:int:min(1)}/effects")]
    public async Task<CreatedActionResult<EffectResponse>> PostCreateEffectAsync(
        [FromServices] NaheulbookExecutionContext executionContext,
        [FromRoute] int subCategoryId,
        CreateEffectRequest request
    )
    {
        try
        {
            var effect = await effectService.CreateEffectAsync(executionContext, subCategoryId, request);
            return effectMapper.ToResponse(effect);
        }
        catch (ForbiddenAccessException ex)
        {
            throw new HttpErrorException(StatusCodes.Status403Forbidden, ex);
        }
    }

    [HttpPost]
    public async Task<CreatedActionResult<EffectSubCategoryResponse>> PostCreateEffectSubCategoryAsync(
        [FromServices] NaheulbookExecutionContext executionContext,
        CreateEffectSubCategoryRequest request
    )
    {
        try
        {
            var effectSubCategory = await effectService.CreateEffectSubCategoryAsync(executionContext, request);
            return effectMapper.ToResponse(effectSubCategory);
        }
        catch (ForbiddenAccessException ex)
        {
            throw new HttpErrorException(StatusCodes.Status403Forbidden, ex);
        }
    }
}