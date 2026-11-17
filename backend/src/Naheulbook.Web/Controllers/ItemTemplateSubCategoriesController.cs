using Microsoft.AspNetCore.Mvc;
using Naheulbook.Core.Features.Item;
using Naheulbook.Core.Features.Shared;
using Naheulbook.Requests.Requests;
using Naheulbook.Responses.Responses;
using Naheulbook.Web.ActionResults;
using Naheulbook.Web.Exceptions;

namespace Naheulbook.Web.Controllers;

[Route("api/v2/itemTemplateSubCategories")]
[ApiController]
public class ItemTemplateSubCategoriesController(IItemTemplateSubCategoryService itemTemplateSubSubCategoryService, IItemTemplateMapper itemTemplateMapper)
    : ControllerBase
{
    [HttpPost]
    public async Task<CreatedActionResult<ItemTemplateSubCategoryResponse>> PostCreateItemTemplateSubCategoryAsync(
        [FromServices] NaheulbookExecutionContext executionContext,
        CreateItemTemplateSubCategoryRequest request
    )
    {
        try
        {
            var itemTemplateSubCategory = await itemTemplateSubSubCategoryService.CreateItemTemplateSubCategoryAsync(executionContext, request);
            return itemTemplateMapper.ToResponse(itemTemplateSubCategory);
        }
        catch (ForbiddenAccessException ex)
        {
            throw new HttpErrorException(StatusCodes.Status403Forbidden, ex);
        }
    }

    [HttpGet("{ItemTemplateSubCategoryName}/itemTemplates")]
    public async Task<ActionResult<List<ItemTemplateResponse>>> GetItemTemplatesBySubCategoryTechNameAsync(
        [FromServices] OptionalNaheulbookExecutionContext executionContext,
        [FromRoute] string itemTemplateSubCategoryName,
        [FromQuery] bool includeCommunityItems = true
    )
    {
        try
        {
            var itemTemplates = await itemTemplateSubSubCategoryService.GetItemTemplatesBySubCategoryTechNameAsync(
                itemTemplateSubCategoryName,
                executionContext.ExecutionExecutionContext?.UserId,
                includeCommunityItems
            );
            return itemTemplates.Select(itemTemplateMapper.ToResponse).ToList();
        }
        catch (ItemTemplateSubCategoryNotFoundException ex)
        {
            throw new HttpErrorException(StatusCodes.Status404NotFound, ex);
        }
    }
}