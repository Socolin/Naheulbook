using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Naheulbook.Core.Features.Effect;
using Naheulbook.Core.Features.Shared;
using Naheulbook.Requests.Requests;
using Naheulbook.Web.ActionResults;
using Naheulbook.Web.Exceptions;
using Naheulbook.Web.Responses;

namespace Naheulbook.Web.Controllers;

[Route("api/v2/effectSubCategories")]
[ApiController]
public class EffectSubCategoriesController(IEffectService effectService, IMapper mapper) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<EffectTypeResponse>>> GetEffectSubCategoriesAsync()
    {
        var effectSubCategories = await effectService.GetEffectSubCategoriesAsync();
        return mapper.Map<List<EffectTypeResponse>>(effectSubCategories);
    }

    [HttpGet("{subCategoryId:int:min(1)}/effects")]
    public async Task<ActionResult<List<EffectResponse>>> GetEffectsAsync(long subCategoryId)
    {
        var effects = await effectService.GetEffectsBySubCategoryAsync(subCategoryId);
        return mapper.Map<List<EffectResponse>>(effects);
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
            return mapper.Map<EffectResponse>(effect);
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
            return mapper.Map<EffectSubCategoryResponse>(effectSubCategory);
        }
        catch (ForbiddenAccessException ex)
        {
            throw new HttpErrorException(StatusCodes.Status403Forbidden, ex);
        }
    }
}