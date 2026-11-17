using Microsoft.AspNetCore.Mvc;
using Naheulbook.Core.Features.Item;
using Naheulbook.Core.Features.Shared;
using Naheulbook.Requests.Requests;
using Naheulbook.Responses.Responses;
using Naheulbook.Web.Exceptions;

namespace Naheulbook.Web.Controllers;

[Route("api/v2/itemTemplateSections")]
[ApiController]
public class ItemTemplateSectionsController(IItemTemplateSectionService itemTemplateSectionService, IItemTemplateMapper itemTemplateMapper)
    : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<ItemTemplateSectionResponse>>> GetItemTemplateSectionsAsync()
    {
        var sections = await itemTemplateSectionService.GetAllSectionsAsync();
        return sections.Select(itemTemplateMapper.ToResponse).ToList();
    }

    [HttpPost]
    public async Task<JsonResult> PostCreateSectionAsync(
        [FromServices] NaheulbookExecutionContext executionContext,
        CreateItemTemplateSectionRequest request
    )
    {
        try
        {
            var itemTemplateSection = await itemTemplateSectionService.CreateItemTemplateSectionAsync(executionContext, request);
            var itemTemplateSectionResponse = itemTemplateMapper.ToResponse(itemTemplateSection);
            return new JsonResult(itemTemplateSectionResponse)
            {
                StatusCode = StatusCodes.Status201Created,
            };
        }
        catch (ForbiddenAccessException ex)
        {
            throw new HttpErrorException(StatusCodes.Status403Forbidden, ex);
        }
    }

    [HttpGet("{SectionId}")]
    public async Task<ActionResult<List<ItemTemplateResponse>>> GetItemTemplatesAsync(
        [FromServices] OptionalNaheulbookExecutionContext executionContext,
        int sectionId
    )
    {
        var sections = await itemTemplateSectionService.GetItemTemplatesBySectionAsync(sectionId, executionContext.ExecutionExecutionContext?.UserId);
        return sections.Select(itemTemplateMapper.ToResponse).ToList();
    }
}