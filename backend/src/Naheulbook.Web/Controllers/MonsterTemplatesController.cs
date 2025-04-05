using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Naheulbook.Core.Features.Monster;
using Naheulbook.Core.Features.Shared;
using Naheulbook.Requests.Requests;
using Naheulbook.Web.Exceptions;
using Naheulbook.Web.Responses;

namespace Naheulbook.Web.Controllers;

[Route("api/v2/monsterTemplates")]
[ApiController]
public class MonsterTemplatesController(IMapper mapper, IMonsterTemplateService monsterTemplateService)
{
    [HttpGet]
    public async Task<ActionResult<List<MonsterTemplateResponse>>> GetMonsterListAsync()
    {
        var monsters = await monsterTemplateService.GetAllMonstersAsync();
        return mapper.Map<List<MonsterTemplateResponse>>(monsters);
    }

    [HttpPost]
    public async Task<ActionResult<MonsterTemplateResponse>> PostAsync(
        [FromServices] NaheulbookExecutionContext executionContext,
        MonsterTemplateRequest request
    )
    {
        try
        {
            var createdMonster = await monsterTemplateService.CreateMonsterTemplateAsync(executionContext, request);

            var result = mapper.Map<MonsterTemplateResponse>(createdMonster);
            return new JsonResult(result) {StatusCode = StatusCodes.Status201Created};
        }
        catch (MonsterSubCategoryNotFoundException ex)
        {
            throw new HttpErrorException(StatusCodes.Status400BadRequest, ex);
        }
        catch (ForbiddenAccessException ex)
        {
            throw new HttpErrorException(StatusCodes.Status403Forbidden, ex);
        }
    }

    [HttpPut("{MonsterTemplateId}")]
    public async Task<ActionResult<MonsterTemplateResponse>> PutAsync(
        [FromServices] NaheulbookExecutionContext executionContext,
        [FromRoute] int monsterTemplateId,
        MonsterTemplateRequest request
    )
    {
        try
        {
            var monster = await monsterTemplateService.EditMonsterTemplateAsync(executionContext, monsterTemplateId, request);

            var result = mapper.Map<MonsterTemplateResponse>(monster);
            return new JsonResult(result);
        }
        catch (MonsterSubCategoryNotFoundException ex)
        {
            throw new HttpErrorException(StatusCodes.Status400BadRequest, ex);
        }
        catch (MonsterTemplateNotFoundException ex)
        {
            throw new HttpErrorException(StatusCodes.Status404NotFound, ex);
        }
        catch (ForbiddenAccessException ex)
        {
            throw new HttpErrorException(StatusCodes.Status403Forbidden, ex);
        }
    }

    [HttpGet("search")]
    public async Task<ActionResult<List<MonsterTemplateResponse>>> GetSearchMonsterTemplateAsync(
        [FromQuery] string filter,
        [FromQuery] int? monsterTypeId,
        [FromQuery] int? monsterSubCategoryId
    )
    {
        var monsterTemplates = await monsterTemplateService.SearchMonsterAsync(filter, monsterTypeId, monsterSubCategoryId);
        return mapper.Map<List<MonsterTemplateResponse>>(monsterTemplates);
    }
}