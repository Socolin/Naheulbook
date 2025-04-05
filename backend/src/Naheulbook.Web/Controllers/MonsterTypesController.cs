using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Naheulbook.Core.Features.Monster;
using Naheulbook.Core.Features.Shared;
using Naheulbook.Requests.Requests;
using Naheulbook.Web.ActionResults;
using Naheulbook.Web.Exceptions;
using Naheulbook.Web.Responses;

namespace Naheulbook.Web.Controllers;

[Route("api/v2/monsterTypes")]
[ApiController]
public class MonsterTypesController(IMapper mapper, IMonsterTypeService monsterTypeService)
{
    [HttpGet]
    public async Task<ActionResult<List<MonsterTypeResponse>>> GetMonsterTypesAsync()
    {
        var monsterTypes = await monsterTypeService.GetMonsterTypesWithCategoriesAsync();
        return mapper.Map<List<MonsterTypeResponse>>(monsterTypes);
    }


    [HttpPost]
    public async Task<CreatedActionResult<MonsterTypeResponse>> PostCreateMonsterTypeAsync(
        [FromServices] NaheulbookExecutionContext executionContext,
        CreateMonsterTypeRequest request
    )
    {
        try
        {
            var monsterType = await monsterTypeService.CreateMonsterTypeAsync(executionContext, request);
            return mapper.Map<MonsterTypeResponse>(monsterType);
        }
        catch (ForbiddenAccessException ex)
        {
            throw new HttpErrorException(StatusCodes.Status403Forbidden, ex);
        }
    }

    [HttpPost("{MonsterTypeId}/subcategories")]
    public async Task<CreatedActionResult<MonsterSubCategoryResponse>> PostCreateMonsterSubCategoryAsync(
        [FromServices] NaheulbookExecutionContext executionContext,
        [FromRoute] int monsterTypeId,
        CreateMonsterSubCategoryRequest request
    )
    {
        try
        {
            var monsterSubCategory = await monsterTypeService.CreateMonsterSubCategoryAsync(executionContext, monsterTypeId, request);
            return mapper.Map<MonsterSubCategoryResponse>(monsterSubCategory);
        }
        catch (ForbiddenAccessException ex)
        {
            throw new HttpErrorException(StatusCodes.Status403Forbidden, ex);
        }
    }
}